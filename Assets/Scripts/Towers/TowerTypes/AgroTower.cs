using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgroTower : ProjectileTower
{
    [SerializeField] Bullet BulletPrefab;
    [SerializeField] TowerStats towerStats;
    [SerializeField] Renderer minionSpawnBounds;
    [SerializeField] float bulletSpeed = 4f;
    [SerializeField] float timeBetweenShots = 0.1f;
    [SerializeField] int shotsAmount = 3;

    [SerializeField] List<Transform> shotPoints;

    private Coroutine fireCoroutine;

    public void Awake()
    {
        Config = towerStats;
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
        EnemySideSummon = towerStats.EnemySideSummon;
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

    private IEnumerator FireBurst(Transform target)
    {
        for (int i = 0; i < shotsAmount; i++)
        {
            GameObject enemy = CheckForEnemyInRange();
            //Bullet bulletInstance = Instantiate(BulletPrefab, transform.position, Quaternion.identity, ProjectileParent.transform);
            Bullet bulletInstance = Instantiate(BulletPrefab, shotPoints[i].position, Quaternion.identity, ProjectileParent.transform);
            if (enemy != null)

                bulletInstance.InitBullet(enemy.transform, bulletSpeed, towerStats.DamagePerLevel[Level]);
            else
            {
                Destroy(bulletInstance);
            }

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    private void Update()
    {
        if (!Alive) { return; }
        if (!RoundManager.GameRunning) { return; }
        GameObject enemy = CheckForEnemyInRange();
        if (enemy != null && CanAttack())
        {
            lastShotTime = Time.time;

            if (fireCoroutine != null)
                StopCoroutine(fireCoroutine);

            fireCoroutine = StartCoroutine(FireBurst(enemy.transform));
        }
    }
}
