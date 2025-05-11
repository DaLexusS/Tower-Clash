using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SummonUnit : MonoBehaviour
{
    [SerializeField] public GameObject minionParent;
    [SerializeField] public GameObject targetBase;
    [SerializeField] public GameObject enemyMinionFolder;

    private void OnEnable()
    {
        SummonUi.onSummonPressed += SpawnUnit;
    }

    private void OnDestroy()
    {
        SummonUi.onSummonPressed -= SpawnUnit;
    }

    public void SpawnUnit(BaseTower tower)
    {
        Vector2 spawnPos = GetRandomPointInsideBounds(tower.MinionSpawnBounds.gameObject);

        MinionBehavior summonClone = Instantiate(tower.Summon, spawnPos, tower.Summon.transform.rotation, tower.PlayerFolder.transform);
        summonClone.IsEnemy = false;
        summonClone.lane = tower.Lane;
        summonClone.targetTower = tower.TargetTower;
        summonClone.targetBase = tower.TargetBase;
        summonClone.enemyFolder = tower.EnemyFolder;
    }

    Vector2 GetRandomPointInsideBounds(GameObject spawner)
    {
        Renderer renderer = spawner.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }

    /*public void SpawnUnit(GameObject spawner, GameObject unit, GameObject lane, GameObject targetTower)
    {
        //TODO Sub from player money the price
        Vector2 spawnPos = GetRandomPointInsideBounds(spawner);

        GameObject cloneMinion = Instantiate(unit, spawnPos, unit.transform.rotation, minionParent.transform);
        MinionBehavior minionBehavior = cloneMinion.GetComponent<MinionBehavior>();
        minionBehavior.IsEnemy = false;
        minionBehavior.targetBase = targetBase;
        minionBehavior.targetTower = targetTower;
        minionBehavior.enemyFolder = enemyMinionFolder;
        minionBehavior.lane = lane;
    }

    Vector2 GetRandomPointInsideBounds(GameObject spawner)
    {
        Renderer renderer = spawner.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }*/
}
