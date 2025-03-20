using UnityEngine;

public class ArrowTower : BaseTower
{
    [SerializeField] GameObject enemyFolder;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject projectileFolder;

    [SerializeField] public TowerStats towerStats;
    [SerializeField] public float testDamage;

    private void Start()
    {
        TowerName = towerStats.TowerName;
        Level = towerStats.Level;
        FireRate = towerStats.FireRate;
        BaseDamage = towerStats.Damage;
        Range = towerStats.Range;
        UpgradeCostPerLevel = towerStats.UpgradeCostPerLevel;
        UpgradeCost = UpgradeCostPerLevel[Level];
        EnemyFolder = enemyFolder;
        testDamage = BaseDamage;
    }

    public override void Upgrade()
    {
        base.Upgrade();

        if (Level > UpgradeCostPerLevel.Count - 1){ return; }
        UpgradeCost = UpgradeCostPerLevel[Level];
        BaseDamage += 10;
        testDamage = BaseDamage;
    }

    protected override void SpawnBullet(Transform target)
    {
        if (arrowPrefab == null)
        {
            Debug.LogError($"ArrowTower: Bullet prefab is not assigned for {TowerName}!");
            return;
        }

        GameObject bulletInstance = Instantiate(arrowPrefab, transform.position, Quaternion.identity, projectileFolder.transform);
        Bullet bulletScript = bulletInstance.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.InitBullet(target, 4f, BaseDamage, this);
        }
    }

    protected override void Attack(Transform target)
    {
        //Debug.Log($"{TowerName} fires an arrow at {target.name} and deals {BaseDamage} damage!");

    }
}
