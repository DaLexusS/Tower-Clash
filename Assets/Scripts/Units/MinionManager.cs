using UnityEngine;

public class MinionManager : MonoBehaviour
{
    [SerializeField] public SummonBase Minion;
    [SerializeField] public GameObject unitParent;

    [SerializeField] public GameObject playerBase;
    [SerializeField] public GameObject enemyBase;

    [SerializeField] public GameObject playerUnitsParent;
    [SerializeField] public GameObject enemyUnitsParent;

    [SerializeField] public Color playerColor;
    [SerializeField] public Color enemyColor;

    public void SpawnMinion(BaseTower tower)
    {
        Vector2 spawnPos = GetRandomPointInsideBounds(tower.MinionSpawnBounds);

        SummonBase summonClone = Instantiate(Minion, spawnPos, Minion.transform.rotation, tower.PlayerFolder.transform);
        summonClone.Init(tower);
    }

    Vector2 GetRandomPointInsideBounds(Renderer renderer)
    {
        Bounds bounds = renderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }
}
