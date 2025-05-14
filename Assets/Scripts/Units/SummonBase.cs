using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class SummonBase : MonoBehaviour, IDamageable
{
    public static UnityAction<int> RewardOnSummonDeath;
    public SummonStats SummonStats { get; protected set; }
    public Rigidbody2D Rigid_body { get; protected set; }
    public SpriteRenderer SpriteVisual { get; protected set; }
    public int MaxHealth { get; protected set; }
    public int Health { get; protected set; }
    public int Damage { get; protected set; }
    public int Value { get; protected set; }
    public float AttackRange { get; protected set; }
    public float WalkSpeed { get; protected set; }
    public FirstAttackDelayRange FirstAttackCooldown { get; protected set; }
    public float PreAttackTime { get; protected set; }
    public float AttackCoolDown { get; protected set; }

    public bool IsAlive = true;
    public bool IsEnemy = false;

    private TowerHealthManager EnemyTowerHealthManager;
    private BaseHealthManager EnemyBaseHealthManager;

    private SummonBase targetEnemy;
    private GameObject TargetBase;
    private GameObject TargetTower;
    private GameObject EnemyParent;
    private GameObject PlayerParent;
    public GameObject Lane;

    private float lastAttackTime = 0f;
    private float firstAttackDelayTime;
    private bool isStopped = false;
    private bool Loaded = false;
    private bool hasAttackedOnce = false;
    private bool isAttacking = false;

    public virtual void Init(BaseTower towerData)
    {
        TargetBase = towerData.TargetBase;
        TargetTower = towerData.TargetTower;
        Lane = towerData.Lane;
        EnemyParent = towerData.EnemyFolder;
        PlayerParent = towerData.PlayerFolder;

        EnemyTowerHealthManager = TargetTower?.GetComponent<TowerHealthManager>();
        EnemyBaseHealthManager = TargetBase?.GetComponent<BaseHealthManager>();

        if (EnemyTowerHealthManager == null)
        {
            Debug.LogWarning($"[Init] TowerHealthManager is MISSING on {TargetTower?.name}");
        }

        if (EnemyBaseHealthManager == null)
        {
            Debug.LogWarning($"[Init] BaseHealthManager is MISSING on {TargetBase?.name}");
        }

        firstAttackDelayTime = Time.time + Random.Range(FirstAttackCooldown.min, FirstAttackCooldown.max);
        Loaded = true;
    }

    private void Update()
    {
        if (!Loaded) return;

        targetEnemy = CheckForEnemyInRange();

        Vector3 direction = GetTargetDirection();

        Rigid_body.velocity = direction * WalkSpeed;

        Attack();
    }

    private Vector3 GetTargetDirection()
    {
        if (isStopped) return Vector3.zero;

        Vector3 targetPosition;

        if (targetEnemy)
        {
            targetPosition = targetEnemy.transform.position;
        }
        else if (EnemyTowerHealthManager != null && EnemyTowerHealthManager.isAlive)
        {
            targetPosition = TargetTower.transform.position;
        }
        else if (EnemyBaseHealthManager != null && EnemyBaseHealthManager.isAlive)
        {
            targetPosition = TargetBase.transform.position;
        }
        else
        {
            return Vector3.zero;
        }

        return (targetPosition - transform.position).normalized;
    }

    private SummonBase CheckForEnemyInRange()
    {
        SummonBase closestEnemy = null;
        Vector3 currentPosition = transform.position;
        float closestDistance = AttackRange;

        if (EnemyParent == null) return null;

        foreach (Transform child in EnemyParent.transform)
        {
            SummonBase enemy = child.GetComponent<SummonBase>();
            if (enemy == null) continue;

            if (enemy.Lane != Lane) continue;

            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance <= AttackRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void Attack()
    {
        if (isAttacking) return;

        if (!hasAttackedOnce)
        {
            if (Time.time < firstAttackDelayTime) return;
            hasAttackedOnce = true;
        }
        else
        {
            if (Time.time - lastAttackTime < AttackCoolDown) return;
        }

        StartCoroutine(PreAttackDelay());
    }

    private IEnumerator PreAttackDelay()
    {
        isAttacking = true;
        yield return new WaitForSeconds(PreAttackTime);

        if (!IsAlive)
        {
            isAttacking = false;
            yield break;
        }

        if (targetEnemy != null && targetEnemy.IsAlive &&
            Vector3.Distance(transform.position, targetEnemy.transform.position) <= AttackRange)
        {
            targetEnemy.GetComponent<IDamageable>()?.TakeDamage(Damage);
        }
        else if (EnemyTowerHealthManager != null && EnemyTowerHealthManager.isAlive &&
                 Vector3.Distance(transform.position, TargetTower.transform.position) <= AttackRange)
        {
            EnemyTowerHealthManager.TakeDamage(Damage);
        }
        else if (EnemyBaseHealthManager != null && EnemyBaseHealthManager.isAlive &&
                 Vector3.Distance(transform.position, TargetBase.transform.position) <= AttackRange)
        {
            EnemyBaseHealthManager.TakeDamage(Damage);
        }

        lastAttackTime = Time.time;
        isAttacking = false;
    }


    public virtual void TakeDamage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);

        if (Health <= 0) { OnDied(); }
    }

    public virtual void OnDied()
    {
        Destroy(gameObject);

        if (IsEnemy)
        {
            RewardOnSummonDeath.Invoke(Value);
        }

        IsAlive = false;
    }

    public virtual void ColorSummon(Color color)
    {
        SpriteVisual.color = color;
    }
}
