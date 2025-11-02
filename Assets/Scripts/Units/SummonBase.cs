using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System.Collections;

public abstract class SummonBase : MonoBehaviour, IDamageable
{
    [SerializeField] HealthBarUi healthBarUi;
    public static UnityAction<int> RewardOnSummonDeath;
    public static UnityAction<int> RewardOnSummonDeathEnemy;
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
    public Animator SummonAnimator { get; protected set; }

    public bool IsAlive = true;
    public bool IsEnemy = false;
    public GameObject Lane;

    public TowerHealthManager EnemyTowerHealthManager;
    public BaseHealthManager EnemyBaseHealthManager;
    private GameObject TargetBase;
    private GameObject TargetTower;
    private GameObject EnemyParent;
    private GameObject PlayerParent;

    public SummonBase targetEnemy;
    public GameObject currentTarget;
    public Vector2 movementDirection;
    public float lastAttackTime = 0f;
    public float firstAttackDelayTime;
    public bool hasAttackedOnce = false;
    public bool isAttacking = false;
    public Coroutine attackCoroutine;
    public bool initialized = false;

    private bool frozen = false;

    protected bool IsValidLevel<T>(List<T> list)
    {
        return list != null && Level >= 0 && Level < list.Count;
    }

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

        EnemyTowerHealthManager = TargetTower.GetComponent<TowerHealthManager>();
        EnemyBaseHealthManager = TargetBase.GetComponent<BaseHealthManager>();

        firstAttackDelayTime = Time.time + Random.Range(FirstAttackCooldown.min, FirstAttackCooldown.max);
        initialized = true;

        healthBarUi.InitHealth(Health);
    }

    void Update()
    {
        if (!initialized || !IsAlive || !RoundManager.GameRunning)
        {
            if (!frozen)
            {
                frozen = true;
                Rigid_body.constraints = RigidbodyConstraints2D.FreezeAll;
            }

            return;
        }

        if (currentTarget != null)
        {
            SummonBase enemy = currentTarget.GetComponent<SummonBase>();
            if (enemy != null && !enemy.IsAlive)
            {
                currentTarget = null;
                targetEnemy = null;
            }
        }

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
            Vector3 point = GetClosestPointOnTarget(currentTarget);
            movementDirection = (point - transform.position).normalized;
        }
        else if (EnemyTowerHealthManager != null && EnemyTowerHealthManager.isAlive)
        {
            currentTarget = TargetTower;
            Vector3 point = GetClosestPointOnTarget(currentTarget);
            movementDirection = (point - transform.position).normalized;
        }
        else if (EnemyBaseHealthManager != null && EnemyBaseHealthManager.isAlive)
        {
            currentTarget = TargetBase;
            Vector3 point = GetClosestPointOnTarget(currentTarget);
            movementDirection = (point - transform.position).normalized;
        }
        else
        {
            currentTarget = null;
            movementDirection = Vector2.zero;
        }
    }

    private SummonBase FindClosestEnemyInLane()
    {
        if (EnemyParent == null || !IsValidLevel(AttackRange)) return null;

        SummonBase closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform child in EnemyParent.transform)
        {
            SummonBase enemy = child.GetComponent<SummonBase>();
            if (enemy == null || !enemy.IsAlive || enemy.Lane != Lane) continue;

            float distance = Vector3.Distance(currentPosition, GetClosestPointOnTarget(enemy.gameObject));
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
        if (Rigid_body == null || !IsValidLevel(WalkSpeed)) return;

        if (!isAttacking && movementDirection != Vector2.zero)
        {
            Rigid_body.velocity = movementDirection * WalkSpeed[Level];

            if (SummonAnimator != null)
            {
                if (SummonAnimator.GetBool("Idle"))
                {
                    SummonAnimator.SetBool("Idle", false);
                }
            }
        }
        else
        {

            if (SummonAnimator != null)
            {
                SummonAnimator.SetBool("Idle", true);
            }

            Rigid_body.velocity = Vector2.zero;
        }
    }

    private void HandleAttack()
    {
        if (isAttacking || currentTarget == null || !IsValidLevel(AttackRange)) return;
        if (!hasAttackedOnce && Time.time < firstAttackDelayTime) return;
        if (Time.time < lastAttackTime) return;

        float distanceToTarget = Vector3.Distance(transform.position, GetClosestPointOnTarget(currentTarget));
        if (distanceToTarget > AttackRange[Level]) return;

        if (attackCoroutine != null) StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(PerformAttack());
    }

    public virtual IEnumerator PerformAttack()
    {
        if (!IsValidLevel(PreAttackTime) || !IsValidLevel(AttackCoolDown)) yield break;

        if (Time.time < lastAttackTime)
            yield break;

        isAttacking = true;
        hasAttackedOnce = true;

        Vector2 originalVelocity = Rigid_body.velocity;
        Rigid_body.velocity = Vector2.zero;

        yield return new WaitForSeconds(PreAttackTime[Level]);

        if (!IsAlive)
        {
            ResetAfterAttack(originalVelocity);
            yield break;
        }

        float distanceToTarget = GetDistanceToTarget();
        if (distanceToTarget <= AttackRange[Level])
        {
            DoAttack();
            lastAttackTime = Time.time;
        }

        yield return new WaitForSeconds(AttackCoolDown[Level]);

        ResetAfterAttack(originalVelocity);
    }

    public virtual void DoAttack()
    {
        if (currentTarget == null || !IsValidLevel(Damage) || !IsValidLevel(AttackRange)) return;

        float distance = Vector3.Distance(transform.position, GetClosestPointOnTarget(currentTarget));
        if (distance <= AttackRange[Level])
        {
            if (SummonAnimator != null)
            {
               SummonAnimator.SetTrigger("Attack");
            }

            IDamageable damageable = currentTarget.GetComponent<IDamageable>();
            damageable.TakeDamage(Damage[Level]);
        }
    }

    public void ResetAfterAttack(Vector2 originalVelocity)
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

        healthBarUi.UpdateHealth(Health);

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
        else
        {
            RewardOnSummonDeathEnemy?.Invoke(Value);
        }
        if (SummonAnimator != null)
        {
            SummonAnimator.SetTrigger("Death");
        }

        StartCoroutine(DestroyAfterDelay(2f));
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        if (IsValidLevel(AttackRange))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, AttackRange[Level]);
        }

        if (currentTarget != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, GetClosestPointOnTarget(currentTarget));
        }
    }

    protected float GetDistanceToTarget()
    {
        if (currentTarget == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, GetClosestPointOnTarget(currentTarget));
    }

    private void OnDrawGizmos()
    {
        if (IsValidLevel(AttackRange))
        {
            Gizmos.color = new Color(1, 0, 0, 0.3f);
            Gizmos.DrawWireSphere(transform.position, AttackRange[Level]);
        }
    }

    public virtual void ColorSummon(Color color)
    {
        SpriteVisual.color = color;
    }

    private Vector3 GetClosestPointOnTarget(GameObject target)
    {
        if (target == null) return target.transform.position;
        Collider2D col = target.GetComponent<Collider2D>();
        return col != null ? col.ClosestPoint(transform.position) : target.transform.position;
    }
}
