using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MinionBehavior : MonoBehaviour, IDamageable
{
    [SerializeField] public MinionStats minionStats;
    [SerializeField] public GameObject targetTower;
    [SerializeField] public GameObject targetBase;
    [SerializeField] public GameObject enemyFolder;

    public int health;
    public int maxHealth;
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public float walkSpeed;

    private float lastAttackTime = 0f;

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
    }

    private Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
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

        if (targetEnemy)
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

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, walkSpeed * Time.deltaTime);

        Vector3 direction = (targetPosition - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;

        lastAttackTime = Time.time;

        if (targetEnemy != null && Vector3.Distance(transform.position, targetEnemy.transform.position) <= attackRange)
        {
            Debug.Log("aaaa");
            targetEnemy.GetComponent<IDamageable>()?.TakeDamage((int)damage);
        }
        else if (enemyTowerHealthManager != null && enemyTowerHealthManager.isAlive &&
                 Vector3.Distance(transform.position, targetTower.transform.position) <= attackRange)
        {
            enemyTowerHealthManager.TakeDamage((int)damage);
        }
        else if (baseHealthManager != null && baseHealthManager.isAlive &&
                 Vector3.Distance(transform.position, targetBase.transform.position) <= attackRange)
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
        Destroy(gameObject);
    }
}
