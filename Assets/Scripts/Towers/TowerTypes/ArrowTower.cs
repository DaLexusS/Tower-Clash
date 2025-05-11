using UnityEngine;

public class ArrowTower : BaseTower
{
    [SerializeField] Bullet arrowPrefab;
    [SerializeField] public TowerStats towerStats;
    [SerializeField] public Renderer minionSpawnBounds;

    public void Awake()
    {
        TowerName = towerStats.TowerName;
        Level = towerStats.Level;
        MinionSpawnTimeCooldown = towerStats.MinionSpawnTimeCooldown;

        FireRate = towerStats.FireRatePerLevel;
        BaseDamage = towerStats.DamagePerLevel;
        Range = towerStats.RangePerLevel;
        UpgradeCostPerLevel = towerStats.UpgradeCostPerLevel;

        MinionSpawnBounds = minionSpawnBounds;

        SummonIcon = towerStats.SummonIcon;
        SummonPrice = towerStats.SummonPrice;
        Summon = towerStats.Summon;
    }

    public override void Upgrade()
    {
        base.Upgrade();

        if (Level > UpgradeCostPerLevel.Count - 1){ return; }
    }

    protected override void SpawnBullet(Transform target)
    {
        if (arrowPrefab == null)
        {
            Debug.LogError($"ArrowTower: Bullet prefab is not assigned for {TowerName}!");
            return;
        }

        Bullet bulletInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity, ProjectileParent.transform);

        bulletInstance.InitBullet(target, 4f, BaseDamage[Level], this);
    }

    protected override void Attack(Transform target)
    {
        
    }
}
