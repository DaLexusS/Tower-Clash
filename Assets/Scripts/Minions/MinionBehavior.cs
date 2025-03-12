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
    public float walkSpeed;

    private void Awake()
    {
        maxHealth = minionStats.Health;
        health = minionStats.Health;
        damage = minionStats.Damage;
        attackRange = minionStats.AttackRange;
        walkSpeed = minionStats.WalkSpeed;
    }

    private void Update()
    {
        if(enemyFolder == null) { return; }
        if (targetBase && targetTower == null) { return;}

        CheckForEnemyInRange();
    }

    private void CheckForEnemyInRange()
    {

    }

    private void WalkTowardsTarget()
    {

    }
    public void TakeDamage(int damage)
    {
        health = math.max(0, health - damage);

        if (health <= 0) { OnDied(); }
    }
    public void OnDied()
    {

    }
}
