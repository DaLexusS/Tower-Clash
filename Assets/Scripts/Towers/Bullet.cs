using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float Speed;
    private float Damage;
    private Transform targetEnemy;

    public void InitBullet(Transform target, float speed, float damage)
    {
        targetEnemy = target;
        Speed = speed;
        Damage = damage;
        StartCoroutine(MoveToTarget());
    }

    private IEnumerator MoveToTarget()
    {
        Vector3 lastTargetLocation = GetClosestPointOnTarget();
        bool targetWasAlive = true;

        while (true)
        {
            if (targetEnemy != null)
            {
                lastTargetLocation = GetClosestPointOnTarget();
            }
            else
            {
                targetWasAlive = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, lastTargetLocation, Speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, lastTargetLocation) <= 0.05f)
                break;

            yield return null;
        }

        if (targetWasAlive && targetEnemy != null)
        {
            ApplyDamage(targetEnemy);
        }

        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    private void ApplyDamage(Transform target)
    {
        IDamageable damageableTarget = target.GetComponent<IDamageable>();
        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(Mathf.RoundToInt(Damage));
        }
        else
        {
            Debug.LogWarning($"Bullet hit {target.name}, but it does not implement IDamageable!");
        }
    }

    private Vector3 GetClosestPointOnTarget()
    {
        if (targetEnemy == null) return transform.position;
        Collider2D col = targetEnemy.GetComponent<Collider2D>();
        return col != null ? col.ClosestPoint(transform.position) : targetEnemy.position;
    }
}
