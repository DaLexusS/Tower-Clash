using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static EnumLists;

public abstract class SummonBase : MonoBehaviour, IDamageable
{
    [Header("Base References")]
    [SerializeField] protected SummonStats summonStats;
    [SerializeField] protected HealthBarUi healthBarUi;
    [SerializeField] protected SpriteRenderer spriteVisual;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody2D rb;

    public static UnityAction<SummonBase, SummonState> OnStateChanged;
    public static UnityAction<int> RewardOnSummonDeath;
    public static UnityAction<int> RewardOnSummonDeathEnemy;
    public static UnityAction<SummonBase, bool> onEnemyWipedFromList;

    #region Properties
    public int Level { get; protected set; }
    public int Health { get; protected set; }
    public bool IsAlive { get; protected set; } = true;
    [field: SerializeField] public bool IsEnemy { get; set; }
    public GameObject Lane { get; private set; }
    public SummonState CurrentState { get; private set; }
    #endregion

    protected GameObject currentTarget;
    protected bool initialized = false;
    protected bool isAttacking = false;
    protected float lastAttackTime = 0f;

    protected TowerHealthManager enemyTowerHealth;
    protected BaseHealthManager enemyBaseHealth;
    protected GameObject enemyParent;

    [SerializeField] private float attackRandomOffset = 0.05f;
    private float attackStartOffset;

    public virtual void Init(BaseTower towerData)
    {
        Level = towerData.Level;
        Lane = towerData.Lane;
        enemyParent = towerData.EnemyFolder;

        Health = summonStats.HealthPerLevel[Level];

        enemyTowerHealth = towerData.TargetTower.GetComponent<TowerHealthManager>();
        enemyBaseHealth = towerData.TargetBase.GetComponent<BaseHealthManager>();

        healthBarUi?.InitHealth(Health);
        initialized = true;
        attackStartOffset = UnityEngine.Random.Range(0f, attackRandomOffset);
    }

    protected virtual void Update()
    {
        if (!initialized || !IsAlive || !RoundManager.GameRunning)
        {
            if (IsAlive) SetState(SummonState.Idle);
            rb.velocity = Vector2.zero;
            return;
        }

        FindTarget();
        HandleMovement();
    }

    protected virtual void FixedUpdate()
    {
        if (!initialized || !IsAlive || !RoundManager.GameRunning)
        {
            if (IsAlive) SetState(SummonState.Idle);
            rb.velocity = Vector2.zero;
            return;
        }

        HandleAttackLogic();
    }

    #region Combat Logic

    private void HandleAttackLogic()
    {
        if (isAttacking || currentTarget == null) return;

        float dist = GetDistanceToTarget();

        if (dist <= summonStats.GetAttackRange(Level) &&
            Time.time >= lastAttackTime + attackStartOffset)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    public virtual IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        SetState(SummonState.Attacking);

        yield return new WaitForSeconds(
            summonStats.PreAttackTimePerLevel[Level] + attackStartOffset
        );

        if (IsAlive)
        {
            DoAttack();
            lastAttackTime = Time.time + summonStats.AttackCooldownPerLevel[Level];
        }

        isAttacking = false;

        if (IsAlive)
        {
            FindTarget();
            SetState(SummonState.Idle);
        }
    }

    public abstract void DoAttack();

    #endregion

    #region Movement & Targeting

    protected virtual void FindTarget()
    {
        if (isAttacking) return;

        SummonBase closestEnemy = FindClosestEnemyInLane();
        if (closestEnemy != null)
        {
            currentTarget = closestEnemy.gameObject;
        }
        else if (enemyTowerHealth != null && enemyTowerHealth.isAlive)
        {
            currentTarget = enemyTowerHealth.gameObject;
        }
        else if (enemyBaseHealth != null && enemyBaseHealth.isAlive)
        {
            currentTarget = enemyBaseHealth.gameObject;
        }
        else
        {
            currentTarget = null;
        }
    }

    private void HandleMovement()
    {
        if (isAttacking)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        if (currentTarget == null)
        {
            rb.velocity = Vector2.zero;
            SetState(SummonState.Idle);
            return;
        }

        Vector3 targetPos = GetClosestPoint(currentTarget);
        Vector2 direction = (targetPos - transform.position).normalized;

        float dist = Vector2.Distance(transform.position, targetPos);

        if (dist > summonStats.GetAttackRange(Level))
        {
            rb.velocity = direction * summonStats.GetWalkSpeed(Level);
            SetState(SummonState.Moving);
        }
        else
        {
            rb.velocity = Vector2.zero;
            SetState(SummonState.Idle);
        }
    }

    private SummonBase FindClosestEnemyInLane()
    {
        SummonBase closest = null;
        float minCDist = summonStats.GetSpotRange(Level);

        foreach (Transform child in enemyParent.transform)
        {
            SummonBase enemy = child.GetComponent<SummonBase>();
            if (enemy == null || !enemy.IsAlive || enemy.Lane != Lane) continue;

            float d = Vector2.Distance(transform.position, enemy.transform.position);
            if (d < minCDist)
            {
                minCDist = d;
                closest = enemy;
            }
        }
        return closest;
    }

    #endregion

    #region Helpers

    protected void SetState(SummonState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;
        OnStateChanged?.Invoke(this, newState);
    }

    protected float GetDistanceToTarget()
    {
        return currentTarget
            ? Vector2.Distance(transform.position, GetClosestPoint(currentTarget))
            : Mathf.Infinity;
    }

    protected Vector3 GetClosestPoint(GameObject target)
    {
        Collider2D col = target.GetComponent<Collider2D>();
        return col ? (Vector3)col.ClosestPoint(transform.position) : target.transform.position;
    }

    private bool IsValidLevel<T>(List<T> list)
    {
        return list != null && Level >= 0 && Level < list.Count;
    }

    protected bool IsOpponent(GameObject target)
    {
        if (target == null || target == gameObject) return false;

        SummonBase otherSummon = target.GetComponentInParent<SummonBase>();
        if (otherSummon != null)
        {
            bool isDifferentTeam = this.IsEnemy != otherSummon.IsEnemy;
            bool isSameLane = this.Lane == otherSummon.Lane;
            return isDifferentTeam && isSameLane;
        }

        BaseTower tower = target.GetComponentInParent<BaseTower>();
        if (tower != null)
        {
            if (this.IsEnemy)
                return tower.TowerTypeCheck == TowerType.Player;
            else
                return tower.TowerTypeCheck == TowerType.Enemy;
        }

        if (enemyTowerHealth != null &&
            target.transform.IsChildOf(enemyTowerHealth.transform))
            return true;

        if (enemyBaseHealth != null &&
            target.transform.IsChildOf(enemyBaseHealth.transform))
            return true;

        return false;
    }

    public virtual void TakeDamage(int amount)
    {
        SetState(SummonState.Hit);
        Health -= amount;
        healthBarUi?.UpdateHealth(Health);
        if (Health <= 0) Die();
    }

    protected virtual void Die()
    {
        IsAlive = false;
        StopAllCoroutines();
        SetState(SummonState.Died);

        if (IsEnemy)
            RewardOnSummonDeath?.Invoke(summonStats.DeathValue);
        else
            RewardOnSummonDeathEnemy?.Invoke(summonStats.DeathValue);

        Destroy(gameObject, 2f);
        onEnemyWipedFromList?.Invoke(this, IsEnemy);
    }

    public void OnDied()
    {
    }

    #endregion

    #region Gizmos & Debugging

    private void OnDrawGizmos()
    {
        if (summonStats == null) return;

        if (IsValidLevel(summonStats.SpotRangePerLevel))
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position,
                summonStats.GetSpotRange(Level));
        }

        if (IsValidLevel(summonStats.AttackRangePerLevel))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position,
                summonStats.GetAttackRange(Level));
        }

        if (currentTarget != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
        }
    }

    #endregion
}
