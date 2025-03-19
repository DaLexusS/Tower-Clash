using UnityEngine;

public class ArrowTower : BaseTower
{
    [SerializeField] GameObject enemyFolder;
    private void Start()
    {
        TowerName = "Arrow";
        FireRate = 0.5f;
        BaseDamage = 50f;
        Range = 3f;
        UpgradeCost = 150;
        EnemyFolder = enemyFolder;
    }

    protected override void Attack(Transform target)
    {
        Debug.Log($"{TowerName} fires at {target.name}!");
    }
}
