using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

public class MinionBehavior : MonoBehaviour, IDamageable
{
    public static UnityAction<int> onEnemyMinionKilled;
    [SerializeField] public MinionStats minionStats;
    [SerializeField] public GameObject targetTower;
    [SerializeField] public GameObject targetBase;
    [SerializeField] public GameObject enemyFolder;
    [SerializeField] public GameObject lane;
    [SerializeField] public Rigidbody2D minionRigid;
    [SerializeField] public bool IsEnemy = false;

    public int health;
    public int maxHealth;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float walkSpeed;
    public float preAttackTime;
    public FirstAttackDelayRange firstAttackCooldown;
    public bool IsAlive = true;

    private float lastAttackTime = 0f;
    private float firstAttackDelayTime;
    private bool hasAttackedOnce = false;

    private GameObject targetEnemy = null;
    private TowerHealthManager enemyTowerHealthManager;
    private BaseHealthManager baseHealthManager;
    private void Awake()
    {
        maxHealth = minionStats.Health;
        health = minionStats.Health;
        damage = minionStats.Damage;
        attackRange = minionStats.AttackRange;
        walkSpeed = minionStats.WalkSpeed;
        attackCooldown = minionStats.AttackCooldown;
        firstAttackCooldown = minionStats.FirstAttackDelay;
        preAttackTime = minionStats.PreAttackTime;
    }

    private void Start()
    {
        if (targetTower != null)
        {
            enemyTowerHealthManager = targetTower.GetComponent<TowerHealthManager>();
        }

        if (targetBase != null)
        {
            baseHealthManager = targetBase.GetComponent<BaseHealthManager>();
        }

        firstAttackDelayTime = Time.time + UnityEngine.Random.Range(firstAttackCooldown.min, firstAttackCooldown.max);
    }

    private void Update()
    {
        if (targetTower == null)
        {
            return;
        }

        if (enemyTowerHealthManager == null && targetTower != null)
        {
            enemyTowerHealthManager = targetTower.GetComponent<TowerHealthManager>();
        }


        if (targetBase == null || enemyFolder == null) { return; }
        if (!baseHealthManager.isAlive && !enemyTowerHealthManager.isAlive) { return; }

        targetEnemy = CheckForEnemyInRange();
        WalkTowardsTarget();
        Attack();
    }

    private GameObject CheckForEnemyInRange()
    {
        Transform closestEnemy = null;
        float closestDistance = attackRange;
        Vector3 currentPosition = transform.position;

        foreach (Transform enemy in enemyFolder.transform)
        {
            float distance = Vector3.Distance(currentPosition, enemy.position);

            if (distance <= attackRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy != null ? closestEnemy.gameObject : null;
    }

    private void WalkTowardsTarget()
    {
        Vector3 targetPosition;

        if (targetEnemy && targetEnemy.GetComponent<MinionBehavior>().lane == lane)
        {
            targetPosition = targetEnemy.transform.position;
        }
        else if (enemyTowerHealthManager.isAlive)
        {
            targetPosition = targetTower.transform.position;
        }
        else
        {
            targetPosition = targetBase.transform.position;
        }

        Vector3 direction = (targetPosition - transform.position).normalized;

        minionRigid.velocity = direction * walkSpeed;

        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Attack()
    {
        if (!hasAttackedOnce)
        {
            if (Time.time < firstAttackDelayTime) return;
            hasAttackedOnce = true;
        }
        else
        {
            if (Time.time - lastAttackTime < attackCooldown) return;
        }

        lastAttackTime = Time.time;

        StartCoroutine(PreAttackDelay());
    }

    private IEnumerator PreAttackDelay()
    {
        yield return new WaitForSeconds(preAttackTime);

        if (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange && IsAlive)
        {
            targetEnemy.GetComponent<IDamageable>()?.TakeDamage((int)damage);
        }
        else if (enemyTowerHealthManager != null && enemyTowerHealthManager.isAlive &&
                  Vector3.Distance(transform.position, targetTower.transform.position) <= attackRange + 1 && IsAlive)
        {
            enemyTowerHealthManager.TakeDamage((int)damage);
        }
        else if(baseHealthManager != null && baseHealthManager.isAlive &&
                 Vector3.Distance(transform.position, targetBase.transform.position) <= attackRange + 2 && IsAlive)
        {
            baseHealthManager.TakeDamage((int)damage);
        }
    }
    public void TakeDamage(int damage)
    {
        health = math.max(0, health - damage);

        if (health <= 0) { OnDied(); }
    }
    public void OnDied()
    {
        //TEMP
        if(IsEnemy)
        {
            onEnemyMinionKilled.Invoke(5);
        }

        IsAlive = false;
        Destroy(gameObject);
    }
}
