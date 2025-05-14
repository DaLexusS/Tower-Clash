using UnityEngine;
public class Summon_Minion : SummonBase
{
    [SerializeField] public SummonStats summonStats;

    [SerializeField] public Rigidbody2D rigidbody2;

    [SerializeField] public SpriteRenderer visualSprite;

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
        SpriteVisual = visualSprite;

        base.Init(towerData);
    }

    public override void ColorSummon(Color color)
    {
        base.ColorSummon(color);
    }
}
