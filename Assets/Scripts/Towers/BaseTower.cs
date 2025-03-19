using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTower
{
    public string TowerName {  get; protected set; }
    public int Level { get; protected set; } = 1; 
    public float FireRate { get; protected set; }
    public float BaseDamage { get; protected set; }
    public float Range { get; protected set; }
    public int UpgradeCost { get; protected set; }

    private float lastShotTime = -Mathf.Infinity;

    public BaseTower(string name, float fireRate, float damage, float range, int upgradeCost)
    {
        TowerName = name;
        FireRate = fireRate;
        BaseDamage = damage;
        Range = range;
        UpgradeCost = upgradeCost;
    }

    public virtual void Upgrade()
    {
        Level++;
        //Upgrades
        //fire rate *= 1.5
    }

    public Transform FindClosestEnemy(List<Transform> enemies)
    {
        Transform closestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform enemy in enemies)
        {
            float distance = Vector3.Distance(GetTurretPosition(), enemy.position);
            if (distance < minDistance && distance <= Range)
            {
                minDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy;
    }
    public bool CanAttack()
    {
        return Time.time >= lastShotTime + (1f / FireRate);
    }

    public void TryAttack(List<Transform> enemies)
    {
        if (!CanAttack()) return;

        Transform target = FindClosestEnemy(enemies);
        if (target != null)
        {
            lastShotTime = Time.time;
            Attack(target);
        }
    }

    protected abstract void Attack(Transform target);

    protected abstract Vector3 GetTurretPosition();
}
