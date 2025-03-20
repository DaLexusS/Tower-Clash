using UnityEngine;

public abstract class BaseTower : MonoBehaviour
{
    public string TowerName { get; protected set; }
    public int Level { get; protected set; } = 1;
    public float FireRate { get; protected set; }
    public float BaseDamage { get; protected set; }
    public float Range { get; protected set; }
    public int UpgradeCost { get; protected set; }
    public GameObject EnemyFolder { get; protected set; }

    private float lastShotTime = -Mathf.Infinity;

    [SerializeField] protected GameObject bulletPrefab;

    public virtual void Upgrade()
    {
        Level++;
        //TEMP 
        FireRate *= 1.1f;
        BaseDamage *= 1.2f;
        Range *= 1.05f;
        UpgradeCost = (int)(UpgradeCost * 1.5f);
    }

    public GameObject CheckForEnemyInRange()
    {
        if (!EnemyFolder) return null;

        Transform closestEnemy = null;
        float closestDistance = Range;
        Vector3 currentPosition = transform.position;

        foreach (Transform enemy in EnemyFolder.transform)
        {
            float distance = Vector3.Distance(currentPosition, enemy.position);
            if (distance <= Range && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        return closestEnemy != null ? closestEnemy.gameObject : null;
    }

    public bool CanAttack()
    {
        return Time.time >= lastShotTime + FireRate;
    }

    private void Update()
    {
        GameObject enemy = CheckForEnemyInRange();
        if (enemy != null && CanAttack())
        {
            lastShotTime = Time.time;

            SpawnBullet(enemy.transform);
        }
    }

    protected abstract void SpawnBullet(Transform target);

    public void OnBulletHit(Transform target)
    {
        Attack(target);
    }

    protected abstract void Attack(Transform target);

}
