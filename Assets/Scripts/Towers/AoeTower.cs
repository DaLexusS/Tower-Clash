using System.Collections.Generic;
using UnityEngine;

public class AoeTower : BaseTower
{
    public Vector2 aoeBoxSize { get; set; }
    public float aoeDistanceFromTower { get; set; }

    public virtual void Init(float range, float distanceFromTower)
    {
        aoeBoxSize = new Vector2(range, range);
        aoeDistanceFromTower = distanceFromTower;
    }

    public virtual List<SummonBase> CheckForEnemiesInRange()
    {
        if (!EnemyFolder || !IsValidLevel(Range)) return null;

        Vector2 center = (Vector2)transform.position + (Vector2)transform.up * aoeDistanceFromTower;
        Debug.Log(center);

        Collider2D[] hits = Physics2D.OverlapBoxAll(center, aoeBoxSize, 0f);

        List<SummonBase> targetsInRange = new List<SummonBase>();

        foreach (var hit in hits)
        {
            SummonBase summon = hit.transform.parent.GetComponent<SummonBase>();
            if (summon != null && summon.IsAlive && summon.Lane == Lane && summon.IsEnemy)
            {
                targetsInRange.Add(summon);
            }
        }

        return targetsInRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (!IsValidLevel(Range)) return;

        Gizmos.color = Color.yellow;
        Vector2 center = (Vector2)transform.position + (Vector2)transform.up * aoeDistanceFromTower;
        Gizmos.DrawWireCube(center, aoeBoxSize);
    }
}