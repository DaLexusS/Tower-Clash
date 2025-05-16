using UnityEngine;

public class ShotGunTower : BaseTower
{
    [SerializeField] Bullet BulletPrefab;
    [SerializeField] public TowerStats towerStats;
    [SerializeField] public Renderer minionSpawnBounds;

    private float DamageMultiply = 3f;

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
        if (BulletPrefab == null)
        {
            Debug.LogError($"ArrowTower: Bullet prefab is not assigned for {TowerName}!");
            return;
        }

        Bullet bulletInstance = Instantiate(BulletPrefab, transform.position, Quaternion.identity, ProjectileParent.transform);

        float distance = Vector3.Distance(transform.position, target.position);

        float normalizedDistance = Mathf.Clamp01(distance / Range[Level]);

        float inverseDistance = 1 - normalizedDistance;

        float damageMultiplier = Mathf.Lerp(1f, DamageMultiply, inverseDistance);

        float finalDamage = Mathf.RoundToInt(BaseDamage[Level] * damageMultiplier);

        bulletInstance.InitBullet(target, 4f, finalDamage);
    }

    protected override void Attack(Transform target)
    {
        
    }
}
