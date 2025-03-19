using UnityEngine;

public class ArrowTower : BaseTower
{
    [SerializeField] GameObject enemyFolder;
    [SerializeField] GameObject arrowPrefab;
    [SerializeField] GameObject projectileFolder;

    private void Start()
    {
        TowerName = "Arrow";
        FireRate = 0.5f;
        BaseDamage = 50f;
        Range = 10f;
        UpgradeCost = 150;
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
        Debug.Log($"{TowerName} fires an arrow at {target.name} and deals {BaseDamage} damage!");
    }
}
