using UnityEngine;

public class Summon_Shotgun : SummonBase
{
    [Header("Ranged Settings")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float shotgunMultiplier = 3f;

    public override void DoAttack()
    {
        int finalDamage = summonStats.DamagePerLevel[Level];

        if (shotgunMultiplier > 1f)
        {
            float attackRange = summonStats.GetAttackRange(Level);

            float distRatio =
                1f - Mathf.Clamp01(GetDistanceToTarget() / attackRange);

            finalDamage = Mathf.RoundToInt(
                finalDamage * Mathf.Lerp(1f, shotgunMultiplier, distRatio)
            );
        }

        Bullet bullet = Instantiate(
            bulletPrefab,
            transform.position,
            Quaternion.identity
        );

        bullet.InitBullet(
            currentTarget.transform,
            bulletSpeed,
            finalDamage
        );
    }
}
