using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static EnumLists;

public abstract class BaseTower : MonoBehaviour
{
    public string TowerName { get; protected set; }
    public int Level { get; protected set; } = 1;
    public List<float> FireRate { get; protected set; }
    public List<float> BaseDamage { get; protected set; }
    public List<float> Range { get; protected set; }
    public List<int> UpgradeCostPerLevel { get; protected set; }
    public float MinionSpawnTimeCooldown { get; protected set; }
    public GameObject ProjectileParent { get; protected set; }
    public MinionBehavior Summon { get; protected set; }
    public GameObject TargetBase { get; protected set; }
    public GameObject TargetTower { get; protected set; }
    public Renderer MinionSpawnBounds { get; protected set; }
    public Sprite SummonIcon { get; protected set; }
    public int SummonPrice { get; protected set; }
    public TowerType TowerTypeCheck;

    public bool Alive = true;

    [SerializeField] public GameObject EnemyFolder;
    [SerializeField] public GameObject PlayerFolder;
    [SerializeField] public GameObject Lane;

    private float lastShotTime = -Mathf.Infinity;

    public virtual void Upgrade()
    {
        if (Level == UpgradeCostPerLevel.Count)
        {
            return;
        }
        Level++;
    }
    
    public void Init(GameObject lane, GameObject projectileParent, GameObject unitFolder, GameObject playerUnitFolder, GameObject targetBase, GameObject targetTower)
    {
        PlayerFolder = playerUnitFolder;
        EnemyFolder = unitFolder;
        Lane = lane;
        ProjectileParent = projectileParent;
        TargetTower = targetTower;
        TargetBase = targetBase;
    }

    public GameObject CheckForEnemyInRange()
    {
        if (!EnemyFolder) return null;

        Transform closestEnemy = null;
        float closestDistance = Range[Level];
        Vector3 currentPosition = transform.position;

        foreach (Transform enemy in EnemyFolder.transform)
        {
            float distance = Vector3.Distance(currentPosition, enemy.position);
            if (distance <= Range[Level] && distance < closestDistance && enemy.gameObject.GetComponent<MinionBehavior>().lane == Lane)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy != null ? closestEnemy.gameObject : null;
    }

    public bool CanAttack()
    {
        return Time.time >= lastShotTime + FireRate[Level];
    }

    private void Update()
    {
        GameObject enemy = CheckForEnemyInRange();
        if (enemy != null && CanAttack())
        {
            lastShotTime = Time.time;

            SpawnBullet(enemy.transform);
        }
    }

    protected abstract void SpawnBullet(Transform target);

    public void OnBulletHit(Transform target)
    {
        Attack(target);
    }

    protected abstract void Attack(Transform target);
}
