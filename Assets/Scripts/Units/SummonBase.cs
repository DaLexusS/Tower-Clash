using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EnumLists;

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
    public List<float> SpotRange { get; protected set; }
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
    private bool isHitStateActive = false;
    private Coroutine hitCoroutine;
    public SummonState CurrentState { get; private set; }

    public static UnityAction<SummonBase, SummonState> OnStateChanged;

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
            if (IsAlive)
                SetState(SummonState.Idle);

            Freeze();
            return;
        }

        FindTarget();
        HandleMovement();
        HandleAttack();
    }

    private void Freeze()
    {
        if (frozen) return;
        frozen = true;
        Rigid_body.constraints = RigidbodyConstraints2D.FreezeAll;
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

    protected void SetState(SummonState newState)
    {
        if (CurrentState == newState) return;

        CurrentState = newState;
        OnStateChanged?.Invoke(this, newState);
    }

    private SummonBase FindClosestEnemyInLane()
    {
        if (EnemyParent == null || !IsValidLevel(SpotRange)) return null;

        SummonBase closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;

        foreach (Transform child in EnemyParent.transform)
        {
            SummonBase enemy = child.GetComponent<SummonBase>();
            if (enemy == null || !enemy.IsAlive || enemy.Lane != Lane) continue;

            float distance = Vector3.Distance(
                currentPosition,
                GetClosestPointOnTarget(enemy.gameObject)
            );

            if (distance < closestDistance && distance <= SpotRange[Level])
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
            SetState(SummonState.Moving);
        }
        else
        {
            Rigid_body.velocity = Vector2.zero;

            if (!isAttacking)
                SetState(SummonState.Idle);
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
        SetState(SummonState.Attacking);
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

        if (IsAlive)
            SetState(SummonState.Idle);

        Rigid_body.velocity = Vector2.zero;
        attackCoroutine = null;
    }

    public virtual void TakeDamage(int amount)
    {
        if (!IsAlive) return;

        Health = Mathf.Max(0, Health - amount);
        healthBarUi.UpdateHealth(Health);

        if (Health > 0)
        {
            EnterHitState();
        }
        else
        {
            OnDied();
        }
    }

    private void EnterHitState()
    {
        if (isHitStateActive || isAttacking) return;

        if (hitCoroutine != null)
            StopCoroutine(hitCoroutine);

        hitCoroutine = StartCoroutine(HitStateRoutine());
    }

    private IEnumerator HitStateRoutine()
    {
        isHitStateActive = true;
        SetState(SummonState.Hit);

        yield return new WaitForSeconds(0.15f);

        isHitStateActive = false;

        if (IsAlive)
            SetState(SummonState.Idle);
    }

    public virtual void OnDied()
    {
        if (!IsAlive) return;

        IsAlive = false;
        SetState(SummonState.Died);

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        if (IsEnemy)
            RewardOnSummonDeath?.Invoke(Value);
        else
            RewardOnSummonDeathEnemy?.Invoke(Value);

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

        if (IsValidLevel(SpotRange))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, SpotRange[Level]);
        }
    }

    private Vector3 GetClosestPointOnTarget(GameObject target)
    {
        if (target == null) return target.transform.position;
        Collider2D col = target.GetComponent<Collider2D>();
        return col != null ? col.ClosestPoint(transform.position) : target.transform.position;
    }
}
