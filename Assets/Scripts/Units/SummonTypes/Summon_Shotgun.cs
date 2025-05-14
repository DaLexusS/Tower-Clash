using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Summon_Shotgun : SummonBase
{
    [SerializeField] public SummonStats summonStats;

    [SerializeField] public Rigidbody2D rigidbody2;

    public override void Init(BaseTower towerData)
    {
        SummonStats = summonStats;
        Rigid_body = rigidbody2;
        MaxHealth = summonStats.Health;
        Health = summonStats.Health;
        Damage = summonStats.Damage;
        Value = summonStats.DeathValue;
        AttackRange = summonStats.AttackRange;
        WalkSpeed = summonStats.WalkSpeed;
        FirstAttackCooldown = summonStats.FirstAttackDelay;
        PreAttackTime = summonStats.PreAttackTime;

        base.Init(towerData);
    }
}
