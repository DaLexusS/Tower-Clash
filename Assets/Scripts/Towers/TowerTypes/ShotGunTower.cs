using UnityEngine;

public class ShotGunTower : ProjectileTower
{
    [SerializeField] Bullet BulletPrefab;
    [SerializeField] TowerStats towerStats;
    [SerializeField] Renderer minionSpawnBounds;
    [SerializeField] float bulletSpeed = 4f;
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
        Health = towerStats.HealthPerLevel;

        MinionSpawnBounds = minionSpawnBounds;

        SummonIcon = towerStats.SummonIcon;
        SummonPrice = towerStats.SummonPrice;
        Summon = towerStats.Summon;
    }

    public override void Upgrade()
    {
        if (!IsValidLevel(UpgradeCostPerLevel)) return;

        base.Upgrade();
    }

    public override void Init(Bullet bulletPref)
    {
        base.Init(BulletPrefab);
    }

    public override void SpawnBullet(Transform target)
    {
        if (BulletPrefab == null || !IsValidLevel(BaseDamage) || !IsValidLevel(Range)) return;

        Bullet bulletInstance = Instantiate(BulletPrefab, transform.position, Quaternion.identity, ProjectileParent.transform);

        float distance = Vector3.Distance(transform.position, target.position);
        float normalizedDistance = Mathf.Clamp01(distance / Range[Level]);
        float inverseDistance = 1 - normalizedDistance;

        float damageMultiplier = Mathf.Lerp(1f, DamageMultiply, inverseDistance);
        float finalDamage = Mathf.RoundToInt(BaseDamage[Level] * damageMultiplier);

        bulletInstance.InitBullet(target, bulletSpeed, finalDamage);
    }

    private void Update()
    {
        if (!Alive) { return; }
        if (!RoundManager.GameRunning) { return; }
        GameObject enemy = CheckForEnemyInRange();
        if (enemy != null && CanAttack())
        {
            lastShotTime = Time.time;
            SpawnBullet(enemy.transform);
        }
    }
}
