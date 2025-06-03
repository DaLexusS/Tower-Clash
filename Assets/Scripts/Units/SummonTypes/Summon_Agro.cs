using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon_Agro : SummonBase
{
    [SerializeField] SummonStats summonStats;
    [SerializeField] Rigidbody2D rigidbody2;
    [SerializeField] Bullet BulletPrefab;
    [SerializeField] float bulletSpeed = 4f;

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

    public override void DoAttack()
    {
        if(currentTarget == null)
        {
            return;
        }
        float distance = Vector3.Distance(transform.position, currentTarget.transform.position);

        Bullet bullet = Instantiate(BulletPrefab, transform.position, Quaternion.identity);
        bullet.InitBullet(currentTarget.transform, bulletSpeed, this.Damage[Level]);
    }
}
