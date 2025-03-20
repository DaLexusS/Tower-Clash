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
        FireRate = towerStats.FireRate;
        BaseDamage = towerStats.Damage;
        Range = towerStats.Range;
        UpgradeCost = towerStats.UpgradeCost;
        EnemyFolder = enemyFolder;
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
