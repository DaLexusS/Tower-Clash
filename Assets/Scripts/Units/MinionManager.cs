using UnityEngine;

public class MinionManager : MonoBehaviour
{
    [SerializeField] public MinionBehavior Minion;
    [SerializeField] public GameObject unitParent;

    [SerializeField] public GameObject playerBase;
    [SerializeField] public GameObject enemyBase;

    [SerializeField] public GameObject playerUnitsParent;
    [SerializeField] public GameObject enemyUnitsParent;

    [SerializeField] public Color playerColor;
    [SerializeField] public Color enemyColor;

    public void SpawnMinion(Renderer spawnBounds, EnumLists.TowerType towerType, GameObject targetTower, GameObject lane)
    {
        Vector2 spawnPos = GetRandomPointInsideBounds(spawnBounds);

        MinionBehavior minionClone;

        if (towerType == EnumLists.TowerType.Player)
        {
            minionClone = Instantiate(Minion, spawnPos, Minion.transform.rotation, playerUnitsParent.transform);
            minionClone.ColorMinions(playerColor);
            minionClone.IsEnemy = false;
            minionClone.targetBase = enemyBase;
            minionClone.enemyFolder = enemyUnitsParent;
        }
        else
        {
            minionClone = Instantiate(Minion, spawnPos, Minion.transform.rotation, enemyUnitsParent.transform);
            minionClone.ColorMinions(enemyColor);
            minionClone.IsEnemy = true;
            minionClone.targetBase = playerBase;
            minionClone.enemyFolder = playerUnitsParent;
        }

        minionClone.targetTower = targetTower;
        
        minionClone.lane = lane;
    }

    Vector2 GetRandomPointInsideBounds(Renderer renderer)
    {
        Bounds bounds = renderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }
}
