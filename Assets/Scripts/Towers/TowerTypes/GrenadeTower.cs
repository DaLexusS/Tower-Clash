
using System.Collections.Generic;
using UnityEngine;

public class GrenadeTower : AoeTower
{
    [SerializeField] TowerStats towerStats;
    [SerializeField] Renderer minionSpawnBounds;
    [SerializeField] float distanceFromTower = 2f;

    public void Awake()
    {
        TowerName = towerStats.TowerName;
        Level = towerStats.Level;
        MinionSpawnTimeCooldown = towerStats.MinionSpawnTimeCooldown;

        FireRate = towerStats.FireRatePerLevel;
        BaseDamage = towerStats.DamagePerLevel;
        Range = towerStats.RangePerLevel;
        UpgradeCostPerLevel = towerStats.UpgradeCostPerLevel;
        Health = towerStats.HealthPerLevel;

        MinionSpawnBounds = minionSpawnBounds;

        SummonIcon = towerStats.SummonIcon;
        SummonPrice = towerStats.SummonPrice;
        Summon = towerStats.Summon;

        base.Init(Range[Level], distanceFromTower);
    }

    private void Update()
    {
        if (CanAttack())
        {
            lastShotTime = Time.time;

            List<SummonBase> enemies = CheckForEnemiesInRange();

            foreach (var enemy in enemies)
            {
                Debug.Log(BaseDamage[Level]);
                enemy.TakeDamage((int)BaseDamage[Level]);
            }
        }
    }
}
