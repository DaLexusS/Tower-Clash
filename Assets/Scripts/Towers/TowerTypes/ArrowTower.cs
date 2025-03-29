using UnityEngine;

public class ArrowTower : BaseTower
{
    [SerializeField] GameObject enemyFolder;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject projectileFolder;
    [SerializeField] public TowerStats towerStats;

    private void Start()
    {
        TowerName = towerStats.TowerName;
        Level = towerStats.Level;

        FireRate = towerStats.FireRatePerLevel;
        BaseDamage = towerStats.DamagePerLevel;
        Range = towerStats.RangePerLevel;
        UpgradeCostPerLevel = towerStats.UpgradeCostPerLevel;
        EnemyFolder = enemyFolder;
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

        GameObject bulletInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity, projectileFolder.transform);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>(); // Instead of getting component turn bulletinstance into bullet 

        if (bulletScript != null)
        {
            bulletScript.InitBullet(target, 4f, BaseDamage[Level], this);
        }
    }

    protected override void Attack(Transform target)
    {
        //Debug.Log($"{TowerName} fires an arrow at {target.name} and deals {BaseDamage} damage!");
    }
}
