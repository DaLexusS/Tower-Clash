
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    [SerializeField] public GameObject spawnPoint;
    [SerializeField] public GameObject minion;
    [SerializeField] public GameObject minionFolder;
    [SerializeField] public GameObject enemyMinionFolder;
    [SerializeField] public TowerStats towerStats;
    [SerializeField] public GameObject targetTower;
    [SerializeField] public GameObject targetBase;
    [SerializeField] public GameObject lane;

    private float minionSpawnTime;
    private float lastTick;

    private Renderer LaneRenderer;

    private TowerHealthManager towerManager;

    private void Awake()
    {
        minionSpawnTime = towerStats.MinionSpawnTimeCooldown;

        LaneRenderer = spawnPoint.GetComponent<Renderer>();

        lastTick = Time.time + minionSpawnTime;

        towerManager = gameObject.GetComponent<TowerHealthManager>();
    }

    private void Update()
    {
        if (!CanMinionSpawn()) { return; }

        lastTick = Time.time + minionSpawnTime;

        SpawnMinion();
    }

    private void SpawnMinion()
    {
        Vector2 spawnPos = GetRandomPointInsideBounds(LaneRenderer);

        GameObject cloneMinion = Instantiate(minion, spawnPos, minion.transform.rotation, minionFolder.transform);
        MinionBehavior minionBehavior = cloneMinion.GetComponent<MinionBehavior>();

        minionBehavior.targetBase = targetBase;
        minionBehavior.targetTower = targetTower;
        minionBehavior.enemyFolder = enemyMinionFolder;
        minionBehavior.lane = lane;
    }

    private bool CanMinionSpawn()
    {
        if (Time.time < lastTick) { return false; }
        if (!towerManager.isAlive) { return false; }

        return true;
    }

    Vector2 GetRandomPointInsideBounds(Renderer renderer)
    {
        Bounds bounds = renderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }

}
