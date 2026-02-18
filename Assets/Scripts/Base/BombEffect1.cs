using UnityEngine;
using static EnumLists;

public class Bomb : MonoBehaviour
{
    public ExplodeEffect ExplodeEffect;
    public SpriteRenderer bombVisual;

    private Vector3 startPos;
    private Vector3 targetPos;

    private float damage;
    private float radius;

    private float travelTime = 0.6f;
    private float arcHeight = 1.5f;

    private float timer;

    private bool ownerIsEnemy;
    private bool hasExploded;
    private GameObject ownerLane;

    public void Init(Vector3 target, float dmg, float rad, bool isEnemy, GameObject lane)
    {
        startPos = transform.position;
        targetPos = target;
        damage = dmg;
        radius = rad;
        ownerIsEnemy = isEnemy;
        ownerLane = lane;
    }

    private void Update()
    {
        if (hasExploded) return;

        timer += Time.deltaTime;
        float t = timer / travelTime;

        if (t >= 1f)
        {
            transform.position = targetPos;
            Explode();
            return;
        }

        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

        transform.position = pos;
    }

    private void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        // Spawn effect
        if (ExplodeEffect != null)
            Instantiate(ExplodeEffect, transform.position, Quaternion.identity);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            SummonBase other = hit.GetComponentInParent<SummonBase>();

            if (other != null)
            {
                // Only damage enemies on the same lane
                if (other.IsEnemy == ownerIsEnemy) continue;
                if (other.Lane != ownerLane) continue;

                other.TakeDamage((int)damage);
                continue;
            }

            BaseTower tower = hit.GetComponentInParent<BaseTower>();
            if (tower != null)
            {
                // Only damage enemy tower on the same lane
                bool isCorrectTarget = ownerIsEnemy ? tower.TowerTypeCheck == TowerType.Player : tower.TowerTypeCheck == TowerType.Enemy;

                if (isCorrectTarget && tower.Lane == ownerLane)
                {
                    // Hit tower through its health manager
                    TowerHealthManager tm = tower.GetComponent<TowerHealthManager>();
                    if (tm != null)
                    {
                        tm.TakeDamage((int)damage);
                    }
                }
            }
        }

        bombVisual.gameObject.SetActive(false);

        Destroy(gameObject, 2f);
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.yellow;

            Vector3 previousPoint = startPos;

            for (int i = 1; i <= 20; i++)
            {
                float t = i / 20f;

                Vector3 point = Vector3.Lerp(startPos, targetPos, t);
                point.y += Mathf.Sin(t * Mathf.PI) * arcHeight;

                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(targetPos, 0.15f);
        }
    }
}
