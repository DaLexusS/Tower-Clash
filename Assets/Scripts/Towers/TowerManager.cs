using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LaneTowers
{
    public BaseTower playerTower;
    public BaseTower enemyTower;

    

    public LaneTowers(BaseTower player, BaseTower enemy)
    {
        playerTower = player;
        enemyTower = enemy;
    }
}

public class TowerManager : MonoBehaviour
{
    [SerializeField] public MinionManager minionManager;
    [SerializeField] public List<BaseTower> playerTowerList;
    [SerializeField] public List<BaseTower> enemyTowerList;
    [SerializeField] public GameObject enemyMinionParent;
    [SerializeField] public GameObject playerMinionParent;
    [SerializeField] public List<GameObject> Lanes;
    [SerializeField] public GameObject projectileParent;
    [SerializeField] public PlayerManager playerManager;
    [SerializeField] public SummonUi summonUi;
    [SerializeField] public GameObject playerBase;
    [SerializeField] public GameObject enemyBase;

    [SerializeField] public AiEnemy aiEnemy;

    private List<Transform> playerTowerSpawns;
    private List<Transform> enemyTowerSpawns;

    private List<Transform> enemyUnits;
    private List<Transform> playerUnits;

    private List<BaseTower> activeTowers;

    public Dictionary<int, LaneTowers> towerLanes = new Dictionary<int, LaneTowers>();

    private Dictionary<BaseTower, float> towerSpawnTimers = new Dictionary<BaseTower, float>();

    private void OnEnable()
    {
        MinionBehavior.onEnemyWipedFromList += UpdateMinionDeath;
        TowerHealthManager.onTowerDied += TowerDestroyed;
    }

    private void OnDisable()
    {
        MinionBehavior.onEnemyWipedFromList -= UpdateMinionDeath;
        TowerHealthManager.onTowerDied -= TowerDestroyed;
    }

    private void Awake()
    {
        RoundManager.SetGameRunning(true);
        activeTowers = new List<BaseTower>();
        PreLoadSpawnPoints();
        InitTowers();
    }

    private void TowerDestroyed(BaseTower tower)
    {
        if (towerSpawnTimers.ContainsKey(tower))
            towerSpawnTimers.Remove(tower);

        if (activeTowers.Contains(tower))
            activeTowers.Remove(tower);

        foreach (var kvp in towerLanes)
        {
            if (kvp.Value.playerTower == tower)
            {
                kvp.Value.playerTower = null;
                break;
            }
            else if (kvp.Value.enemyTower == tower)
            {
                kvp.Value.enemyTower = null;
                break;
            }
        }
    }

    private void Update()
    {
        if (!RoundManager.GameRunning){ return; }

        foreach (var pair in towerLanes)
        {
            GameObject lane = Lanes[pair.Key];

            BaseTower playerTower = pair.Value.playerTower;
            BaseTower enemyTower = pair.Value.enemyTower;

            if (playerTower != null && Time.time >= towerSpawnTimers[playerTower])
            {
                minionManager.SpawnMinion(playerTower);

                towerSpawnTimers[playerTower] = Time.time + playerTower.MinionSpawnTimeCooldown;
            }

            if (enemyTower != null && Time.time >= towerSpawnTimers[enemyTower])
            {
                minionManager.SpawnMinion(enemyTower);

                towerSpawnTimers[enemyTower] = Time.time + enemyTower.MinionSpawnTimeCooldown;
            }
        }

        UpdateList();
    }

    private void UpdateList()
    {
        foreach ( var tower in activeTowers)
        {
            if (tower.TowerTypeCheck == EnumLists.TowerType.Player)
            {
                tower.EnemyFolder = enemyMinionParent;
            }
            else
            {
                tower.EnemyFolder = playerMinionParent;
            }
        }
    }

    private void UpdateMinionDeath(MinionBehavior minion, bool isEnemy)
    {
        if (isEnemy)
        {
            enemyUnits.Remove(minion.transform);
        }
        else
        {
            playerUnits.Remove(minion.transform);
        }
    }

    private void PreLoadSpawnPoints()
    {
        playerTowerSpawns = new List<Transform>();
        enemyTowerSpawns = new List<Transform>();

        foreach (GameObject lane in Lanes)
        {
            foreach (LaneSpawnPoint point in lane.GetComponentsInChildren<LaneSpawnPoint>())
            {
                if (point.spawnType == EnumLists.SpawnType.Player)
                    playerTowerSpawns.Add(point.transform);
                else if (point.spawnType == EnumLists.SpawnType.Enemy)
                    enemyTowerSpawns.Add(point.transform);
            }
        }
    }

    private void UpdateUi()
    {
        List<BaseTower> realPlayerTowers = GetPlayerTowers();
        summonUi.Init(realPlayerTowers);
    }

    private void InitTowers()
    {
        int laneCount = Mathf.Min(
            playerTowerList.Count, 
            enemyTowerList.Count, 
            playerTowerSpawns.Count, 
            enemyTowerSpawns.Count, 
            Lanes.Count
        );

        for (int i = 0; i < laneCount; i++)
        {
            GameObject lane = Lanes[i];

            Transform playerSpawn = null;
            Transform enemySpawn = null;

            foreach (LaneSpawnPoint point in lane.GetComponentsInChildren<LaneSpawnPoint>())
            {
                if (point.spawnType == EnumLists.SpawnType.Player)
                    playerSpawn = point.transform;
                else if (point.spawnType == EnumLists.SpawnType.Enemy)
                    enemySpawn = point.transform;
            }

            if (playerSpawn == null || enemySpawn == null)
            {
                Debug.LogWarning($"Lane {i} is missing spawn points.");
                continue;
            }

            BaseTower playerTower = Instantiate(playerTowerList[i], playerSpawn.position, Quaternion.identity, lane.transform);
            BaseTower enemyTower = Instantiate(enemyTowerList[i], enemySpawn.position, Quaternion.identity, lane.transform);

            if (playerUnits == null) playerUnits = new List<Transform>();
            if (enemyUnits == null) enemyUnits = new List<Transform>();

            playerTower.Init(lane, projectileParent, enemyMinionParent, playerMinionParent, enemyBase, enemyTower.gameObject);
            enemyTower.Init(lane, projectileParent, playerMinionParent, enemyMinionParent, playerBase, playerTower.gameObject);

            towerLanes[i] = new LaneTowers(playerTower, enemyTower);
            activeTowers.Add(playerTower);
            activeTowers.Add(enemyTower);
            aiEnemy.InitTower(enemyTower);

            towerSpawnTimers[playerTower] = Time.time + playerTower.MinionSpawnTimeCooldown;
            towerSpawnTimers[enemyTower] = Time.time + enemyTower.MinionSpawnTimeCooldown;
        }

        foreach (BaseTower tower in activeTowers)
        {
            if (tower.TowerTypeCheck == EnumLists.TowerType.Player)
            {
                TowerUI towerUI = tower.GetComponent<TowerUI>();
                towerUI.Init(playerManager, tower);
            }
        }

        UpdateUi();
    }

    public List<BaseTower> GetPlayerTowers()
    {
        return activeTowers.FindAll(t => t.TowerTypeCheck == EnumLists.TowerType.Player);
    }


}
