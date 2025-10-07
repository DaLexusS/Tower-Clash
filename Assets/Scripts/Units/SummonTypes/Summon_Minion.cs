using UnityEngine;
public class Summon_Minion : SummonBase
{
    [SerializeField] public SummonStats summonStats;

    [SerializeField] public Rigidbody2D rigidbody2;

    [SerializeField] public SpriteRenderer visualSprite;

    [SerializeField] public Animator animator;

    public override void Init(BaseTower towerData)
    {
        SummonStats = summonStats;
        Rigid_body = rigidbody2;
        Level = summonStats.Level;
        MaxHealth = summonStats.HealthPerLevel;
        Health = summonStats.HealthPerLevel[Level];
        Damage = summonStats.DamagePerLevel;
        Value = summonStats.DeathValue;
        AttackCoolDown = summonStats.AttackCooldownPerLevel;
        AttackRange = summonStats.AttackRangePerLevel;
        WalkSpeed = summonStats.WalkSpeedPerLevel;
        FirstAttackCooldown = summonStats.FirstAttackDelay;
        PreAttackTime = summonStats.PreAttackTimePerLevel;
        SpriteVisual = visualSprite;
        SummonAnimator = animator;

        base.Init(towerData);
    }

    public override void ColorSummon(Color color)
    {
        base.ColorSummon(color);
    }
}
