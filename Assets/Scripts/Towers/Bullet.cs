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
        while (target != null && Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            Vector3 targetPos = target.position;
            targetPos.z = 0;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        if (target != null)
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
