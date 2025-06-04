using UnityEngine;

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

    public void SpawnUnit(BaseTower summonTower, BaseTower laneTower)
    {
        switch (summonTower.spawnFormation) 
        {
            case EnumLists.SpawnFormation.Double:
                SpawnDue(summonTower, laneTower);
                break;
             case EnumLists.SpawnFormation.Triple:
                SpawnTriple(summonTower, laneTower);
                break;
            default:
                SpawnSolo(summonTower, laneTower);
                break;
        }
    }

    private void SpawnSolo(BaseTower tower, BaseTower otherLane)
    {
        Vector2 spawnPos = otherLane.MinionSpawnBounds.transform.position;

        SummonBase summonClone = Instantiate(tower.Summon, spawnPos, tower.Summon.transform.rotation, tower.PlayerFolder.transform);

        summonClone.Init(otherLane);
    }

    private void SpawnDue(BaseTower tower, BaseTower otherLane)
    {
        Transform t = otherLane.MinionSpawnBounds.transform;
        float halfWidth = t.localScale.x * 0.5f;

        float left = t.position.x - halfWidth;
        float right = t.position.x + halfWidth;

        Vector2 leftPoint = new Vector2(left, t.position.y);
        Vector2 rightPoint = new Vector2(right, t.position.y);

        SummonBase summonClone1 = Instantiate(tower.Summon, leftPoint, tower.Summon.transform.rotation, tower.PlayerFolder.transform);
        SummonBase summonClone2 = Instantiate(tower.Summon, rightPoint, tower.Summon.transform.rotation, tower.PlayerFolder.transform);

        summonClone1.Init(otherLane);
        summonClone2.Init(otherLane);
    }

    private void SpawnTriple(BaseTower tower, BaseTower otherLane)
    {
        Transform t = otherLane.MinionSpawnBounds.transform;
        float halfWidth = t.localScale.x * 0.25f;
        float halfHight = t.localScale.y * 0.25f;

        float offsetY =  t.localScale.y * 0.25f;


        float left = t.position.x - halfWidth;
        float right = t.position.x + halfWidth;
        float top = t.position.y + halfHight;

        Vector2 leftPoint = new Vector2(left, t.position.y - offsetY);
        Vector2 rightPoint = new Vector2(right, t.position.y - offsetY);
        Vector2 TopPoint = new Vector2(t.position.x, top);

        SummonBase summonClone1 = Instantiate(tower.Summon, leftPoint, tower.Summon.transform.rotation, tower.PlayerFolder.transform);
        SummonBase summonClone2 = Instantiate(tower.Summon, rightPoint, tower.Summon.transform.rotation, tower.PlayerFolder.transform);
        SummonBase summonClone3 = Instantiate(tower.Summon, TopPoint, tower.Summon.transform.rotation, tower.PlayerFolder.transform);

        summonClone1.Init(otherLane);
        summonClone2.Init(otherLane);
        summonClone3.Init(otherLane);
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
