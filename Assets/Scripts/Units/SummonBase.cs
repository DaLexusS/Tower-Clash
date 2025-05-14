using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class SummonBase : MonoBehaviour, IDamageable
{
    public static UnityAction<int> RewardOnSummonDeath;
    public SummonStats SummonStats { get; protected set; }
    public Rigidbody2D Rigid_body { get; protected set; }
    public SpriteRenderer SpriteVisual { get; protected set; }
    public int Level { get; protected set; }
    public List<int> MaxHealth { get; protected set; }
    public int Health { get; protected set; }
    public List<int> Damage { get; protected set; }
    public int Value { get; protected set; }
    public List<float> AttackRange { get; protected set; }
    public List<float> WalkSpeed { get; protected set; }
    public FirstAttackDelayRange FirstAttackCooldown { get; protected set; }
    public List<float> PreAttackTime { get; protected set; }
    public List<float> AttackCoolDown { get; protected set; }

    public bool IsAlive = true;
    public bool IsEnemy = false;
    public GameObject Lane;

    private TowerHealthManager EnemyTowerHealthManager;
    private BaseHealthManager EnemyBaseHealthManager;
    private GameObject TargetBase;
    private GameObject TargetTower;
    private GameObject EnemyParent;
    private GameObject PlayerParent;

    private SummonBase targetEnemy;
    private GameObject currentTarget;
    private Vector2 movementDirection;
    private float lastAttackTime = 0f;
    private float firstAttackDelayTime;
    private bool hasAttackedOnce = false;
    private bool isAttacking = false;
    private Coroutine attackCoroutine;
    private bool initialized = false;

    void Awake()
    {
        if (Rigid_body == null) Rigid_body = GetComponent<Rigidbody2D>();
        if (SpriteVisual == null) SpriteVisual = GetComponent<SpriteRenderer>();
    }

    public virtual void Init(BaseTower towerData)
    {
        Level = towerData.Level;
        TargetBase = towerData.TargetBase;
        TargetTower = towerData.TargetTower;
        Lane = towerData.Lane;
        EnemyParent = towerData.EnemyFolder;
        PlayerParent = towerData.PlayerFolder;

        EnemyTowerHealthManager = TargetTower?.GetComponent<TowerHealthManager>();
        EnemyBaseHealthManager = TargetBase?.GetComponent<BaseHealthManager>();

        firstAttackDelayTime = Time.time + Random.Range(FirstAttackCooldown.min, FirstAttackCooldown.max);
        initialized = true;
    }

    void Update()
    {
        if (!initialized || !IsAlive) return;

        FindTarget();
        HandleMovement();
        HandleAttack();
    }

    private void FindTarget()
    {
        if (isAttacking) return;

        targetEnemy = FindClosestEnemyInLane();

        if (targetEnemy != null)
        {
            currentTarget = targetEnemy.gameObject;
            movementDirection = (targetEnemy.transform.position - transform.position).normalized;
        }
        else if (EnemyTowerHealthManager != null && EnemyTowerHealthManager.isAlive)
        {
            currentTarget = TargetTower;
            movementDirection = (TargetTower.transform.position - transform.position).normalized;
        }
        else if (EnemyBaseHealthManager != null && EnemyBaseHealthManager.isAlive)
        {
            currentTarget = TargetBase;
            movementDirection = (TargetBase.transform.position - transform.position).normalized;
        }
        else
        {
            currentTarget = null;
            movementDirection = Vector2.zero;
        }
    }

    private SummonBase FindClosestEnemyInLane()
    {
        if (EnemyParent == null) return null;

        SummonBase closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform child in EnemyParent.transform)
        {
            SummonBase enemy = child.GetComponent<SummonBase>();
            if (enemy == null || !enemy.IsAlive || enemy.Lane != Lane) continue;

            float distance = Vector3.Distance(currentPosition, enemy.transform.position);
            if (distance < closestDistance && distance <= AttackRange[Level])
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }

    private void HandleMovement()
    {
        if (Rigid_body == null) return;

        if (!isAttacking && movementDirection != Vector2.zero)
        {
            Rigid_body.velocity = movementDirection * WalkSpeed[Level];
        }
        else
        {
            Rigid_body.velocity = Vector2.zero;
        }
    }

    private void HandleAttack()
    {
        if (isAttacking || currentTarget == null) return;
        if (!hasAttackedOnce && Time.time < firstAttackDelayTime) return;
        if (Time.time < lastAttackTime) return;

        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (distanceToTarget > AttackRange[Level]) return;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(PerformAttack());
    }

    private IEnumerator PerformAttack()
    {
        isAttacking = true;
        hasAttackedOnce = true;

        Vector2 originalVelocity = Rigid_body.velocity;
        Rigid_body.velocity = Vector2.zero;

        yield return new WaitForSeconds(PreAttackTime[Level]);

        if (!IsAlive || currentTarget == null)
        {
            ResetAfterAttack(originalVelocity);
            yield break;
        }

        float currentDistance = Vector3.Distance(transform.position, currentTarget.transform.position);
        if (currentDistance <= AttackRange[Level])
        {
            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            damageable?.TakeDamage(Damage[Level]);
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(AttackCoolDown[Level]);

        ResetAfterAttack(originalVelocity);
    }

    private void ResetAfterAttack(Vector2 originalVelocity)
    {
        isAttacking = false;

        if (IsAlive && currentTarget != null)
        {
            Rigid_body.velocity = originalVelocity;
        }
        else
        {
            Rigid_body.velocity = Vector2.zero;
        }

        attackCoroutine = null;
    }

    public virtual void TakeDamage(int amount)
    {
        Health = Mathf.Max(0, Health - amount);
        if (Health <= 0) OnDied();
    }

    public virtual void OnDied()
    {
        IsAlive = false;

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (IsEnemy)
        {
            RewardOnSummonDeath?.Invoke(Value);
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        // Always draw attack range, regardless of initialization
        if (AttackRange != null && AttackRange.Count > 0)
        {
            int drawLevel = initialized ? Level : 0; // Use level 0 if not initialized
            if (drawLevel < AttackRange.Count)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, AttackRange[drawLevel]);
            }
        }

        // Draw target line if we have one
        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }

    // Add this for constant gizmo drawing (not just when selected)
    private void OnDrawGizmos()
    {
        // Draw a smaller, less prominent range indicator always visible
        if (AttackRange != null && AttackRange.Count > 0)
        {
            int drawLevel = initialized ? Level : 0;
            if (drawLevel < AttackRange.Count)
            {
                Gizmos.color = new Color(1, 0, 0, 0.3f); // Semi-transparent red
                Gizmos.DrawWireSphere(transform.position, AttackRange[drawLevel]);
            }
        }
    }

    public virtual void ColorSummon(Color color)
    {
        SpriteVisual.color = color;
    }
}
