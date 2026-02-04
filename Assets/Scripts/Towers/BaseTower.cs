using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static EnumLists;

public abstract class BaseTower : MonoBehaviour
{
    [SerializeField] TowerHealthManager healthManager;

    public SpawnFormation spawnFormation;
    public string TowerName { get; protected set; }
    public int Level { get; protected set; } = 1;
    public List<int> Health { get; protected set; }
    public List<float> FireRate { get; protected set; }
    public List<float> BaseDamage { get; protected set; }
    public List<float> Range { get; protected set; }
    public List<int> UpgradeCostPerLevel { get; protected set; }
    public float MinionSpawnTimeCooldown { get; protected set; }
    public GameObject ProjectileParent { get; protected set; }
    public SummonBase Summon { get; protected set; }
    public SummonBase EnemySideSummon { get; protected set; }
    [SerializeField] public GameObject TargetBase { get; protected set; }
    [SerializeField] public GameObject TargetTower { get; protected set; }
    public Renderer MinionSpawnBounds { get; protected set; }
    public Sprite SummonIcon { get; protected set; }
    public int SummonPrice { get; protected set; }

    public UnityEvent<int> OnUpgraded;

    public TowerType TowerTypeCheck;
    public TowerStats Config { get; protected set; }

    public bool Alive { get; protected set; } = true;

    [SerializeField] public GameObject EnemyFolder;
    [SerializeField] public GameObject PlayerFolder;
    [SerializeField] public GameObject Lane;

    public float lastShotTime = -Mathf.Infinity;

    public void KillTower()
    {
        Alive = false;
    }

    public virtual void Upgrade()
    {
        if (Health == null || Level >= Health.Count - 1)
        {
            Debug.LogError($"{TowerName} Upgrade failed: Health list is null or too short.");
            return;
        }

        Level++;
        healthManager.UpgradeHealth(Health[Level]);

        OnUpgraded.Invoke(Level);
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
    public bool CanAttack()
    {
        return Time.time >= lastShotTime + FireRate[Level];
    }

    protected bool IsTargetEnemy(SummonBase summon)
    {
        if (summon == null || !summon.IsAlive) return false;

        if (TowerTypeCheck == TowerType.Player)
            return summon.IsEnemy;

        
        if (TowerTypeCheck == TowerType.Enemy)
            return !summon.IsEnemy;

        return false;
    }
    protected bool IsValidLevel<T>(List<T> list)
    {
        return list != null && Level >= 0 && Level < list.Count;
    }
}
