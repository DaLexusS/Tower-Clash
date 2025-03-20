using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private BaseTower tower;
    private float bulletDamage;
    private Transform targetEnemy;

    public void InitBullet(Transform target, float speed, float damage, BaseTower parentTower)
    {
        tower = parentTower;
        bulletDamage = damage;
        targetEnemy = target;
        StartCoroutine(MoveToTarget(target, speed));
    }

    private IEnumerator MoveToTarget(Transform target, float speed)
    {
        Vector3 lastTargetLocation = target.position;
        bool targetWasAlive = true;

        while (true)
        {
            if (target != null)
            {
                lastTargetLocation = target.position;
            }
            else
            {
                targetWasAlive = false;
            }

            transform.position = Vector3.MoveTowards(transform.position, lastTargetLocation, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, lastTargetLocation) <= 0.05f)
                break;

            yield return null;
        }

        if (targetWasAlive && target != null)
        {
            ApplyDamage(target);
            tower.OnBulletHit(target);
        }

        Destroy(gameObject);
    }


    private void ApplyDamage(Transform target)
    {
        IDamageable damageableTarget = target.GetComponent<IDamageable>();

        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage((int)bulletDamage);
        }
        else
        {
            Debug.LogWarning($"Bullet hit {target.name}, but it does not implement IDamageable!");
        }
    }
}
