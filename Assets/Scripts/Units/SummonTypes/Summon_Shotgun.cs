using System.Collections;
using UnityEngine;

public class Summon_Shotgun : SummonBase
{
    [SerializeField] public SummonStats summonStats;
    [SerializeField] public Rigidbody2D rigidbody2;
    [SerializeField] public Bullet BulletPrefab;

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

        base.Init(towerData);
    }

    public override IEnumerator PerformAttack()
    {
        if (!IsValidLevel(PreAttackTime) || !IsValidLevel(AttackCoolDown) || !IsValidLevel(Damage) || !IsValidLevel(AttackRange))
            yield break;

        isAttacking = true;
        hasAttackedOnce = true;

        Vector2 originalVelocity = Rigid_body.velocity;
        Rigid_body.velocity = Vector2.zero;

        yield return new WaitForSeconds(PreAttackTime[Level]);

        if (!IsAlive || currentTarget == null)
        {
            ResetAfterAttack(originalVelocity);
            yield break;
        }

        float maxRange = AttackRange[Level];
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (distance <= maxRange && IsValidLevel(Damage))
        {
            float t = distance / maxRange;
            float damageMultiplier = Mathf.Lerp(DamageMultiply, 1f, t);

            int baseDamage = Damage[Level];
            int finalDamage = Mathf.RoundToInt(baseDamage * damageMultiplier);

            Bullet bulletInstance = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
            bulletInstance.InitBullet(currentTarget.transform, 12f, finalDamage);
        }

        lastAttackTime = Time.time;
        yield return new WaitForSeconds(AttackCoolDown[Level]);

        ResetAfterAttack(originalVelocity);
    }
}
