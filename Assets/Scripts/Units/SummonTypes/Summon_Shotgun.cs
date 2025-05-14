using UnityEngine;

public class Summon_Shotgun : SummonBase
{
    [SerializeField] public SummonStats summonStats;

    [SerializeField] public Rigidbody2D rigidbody2;

    public override void Init(BaseTower towerData)
    {
        SummonStats = summonStats;
        Rigid_body = rigidbody2;
        Level = summonStats.Level;
        MaxHealth = summonStats.HealthPerLevel;
        Health = summonStats.HealthPerLevel[Level];
        Damage = summonStats.DamagePerLevel;
        Value = summonStats.DeathValue;
        AttackRange = summonStats.AttackRangePerLevel;
        AttackCoolDown = summonStats.AttackCooldownPerLevel;
        WalkSpeed = summonStats.WalkSpeedPerLevel;
        FirstAttackCooldown = summonStats.FirstAttackDelay;
        PreAttackTime = summonStats.PreAttackTimePerLevel;

        base.Init(towerData);
    }
}
