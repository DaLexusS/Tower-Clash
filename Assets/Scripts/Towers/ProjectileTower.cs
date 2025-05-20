using UnityEngine;

public class ProjectileTower : BaseTower
{
    public Bullet BaseBulletPrefab { get; set; }

    private Transform currentTarget;

    public virtual void Init(Bullet bulletPref)
    {
        BaseBulletPrefab = bulletPref;
    }

    public virtual void SpawnBullet(Transform target)
    {
        if (BaseBulletPrefab == null || !IsValidLevel(BaseDamage)) return;

        Bullet bulletInstance = Instantiate(BaseBulletPrefab, transform.position, Quaternion.identity, ProjectileParent.transform);
        float distance = Vector3.Distance(transform.position, target.position);
        bulletInstance.InitBullet(target, 4f, BaseDamage[Level]);
    }

    public virtual GameObject CheckForEnemyInRange()
    {
        if (!EnemyFolder || !IsValidLevel(Range)) return null;

        Transform closestEnemy = null;
        float closestDistance = Range[Level];
        Vector3 currentPosition = transform.position;

        foreach (Transform enemy in EnemyFolder.transform)
        {
            var summon = enemy.GetComponent<SummonBase>();
            if (summon == null || summon.Lane != Lane) continue;

            float distance = Vector3.Distance(currentPosition, enemy.position);
            if (distance <= Range[Level] && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        currentTarget = closestEnemy;
        return closestEnemy != null ? closestEnemy.gameObject : null;
    }

    private void OnDrawGizmosSelected()
    {
        if (IsValidLevel(Range))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Range[Level]);
        }

        if (currentTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, currentTarget.position);
        }
    }
}
