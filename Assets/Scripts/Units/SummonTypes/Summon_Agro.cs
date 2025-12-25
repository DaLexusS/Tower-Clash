using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Summon_Agro : SummonBase
{
    [Header("Ranged Settings")]
    [SerializeField] private Bullet bulletPrefab;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] private float shotgunMultiplier = 1f;

    public override void DoAttack()
    {
        int finalDamage = summonStats.DamagePerLevel[Level];

        if (shotgunMultiplier > 1f)
        {
            float distRatio = 1f - Mathf.Clamp01(GetDistanceToTarget() / summonStats.AttackRangePerLevel[Level]);
            finalDamage = Mathf.RoundToInt(finalDamage * Mathf.Lerp(1f, shotgunMultiplier, distRatio));
        }

        Bullet bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        bullet.InitBullet(currentTarget.transform, bulletSpeed, finalDamage);
    }
}
