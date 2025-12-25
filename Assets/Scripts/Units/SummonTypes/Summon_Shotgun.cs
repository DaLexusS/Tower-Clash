
using UnityEngine;

public class Summon_Shotgun : SummonBase
{
    [SerializeField] SummonStats summonStats;
    [SerializeField] Rigidbody2D rigidbody2;
    [SerializeField] Bullet BulletPrefab;
    [SerializeField] float bulletSpeed = 4f;

    private float DamageMultiply = 3f;

    public override void Init(BaseTower towerData)
    {
        SummonStats = summonStats;
        Rigid_body = rigidbody2;
        Level = summonStats.Level;

        MaxHealth = summonStats.HealthPerLevel;
        if (IsValidLevel(MaxHealth)) Health = MaxHealth[Level]; else Health = 100;

        Damage = summonStats.DamagePerLevel;
        Value = summonStats.DeathValue;
        AttackRange = summonStats.AttackRangePerLevel;
        AttackCoolDown = summonStats.AttackCooldownPerLevel;
        WalkSpeed = summonStats.WalkSpeedPerLevel;
        FirstAttackCooldown = summonStats.FirstAttackDelay;
        PreAttackTime = summonStats.PreAttackTimePerLevel;
        SpotRange = summonStats.SpotRangePerLevel;

        base.Init(towerData);
    }

    public override void DoAttack()
    {
        if (currentTarget == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);
        float t = Mathf.Clamp01(distance / AttackRange[Level]);
        float damageMultiplier = Mathf.Lerp(1f, DamageMultiply, 1f - t);
        int finalDamage = Mathf.RoundToInt(Damage[Level] * damageMultiplier);

        Bullet bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        bullet.InitBullet(currentTarget.transform, bulletSpeed, finalDamage);
    }
}
