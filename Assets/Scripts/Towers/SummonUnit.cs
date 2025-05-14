using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SummonUnit : MonoBehaviour
{
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
        SummonBase summonClone = Instantiate(tower.Summon, spawnPos, tower.Summon.transform.rotation, tower.PlayerFolder.transform);

        summonClone.Init(tower);
    }

    Vector2 GetRandomPointInsideBounds(GameObject spawner)
    {
        Renderer renderer = spawner.GetComponent<Renderer>();
        Bounds bounds = renderer.bounds;
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        return new Vector2(randomX, randomY);
    }
}
