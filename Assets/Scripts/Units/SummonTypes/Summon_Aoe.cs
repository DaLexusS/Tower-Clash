using UnityEngine;

public class Summon_Aoe : SummonBase
{
    [SerializeField] SummonStats summonStats;
    [SerializeField] Rigidbody2D rigidbody2;

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
        if (!IsValidLevel(Damage) || !IsValidLevel(AttackRange)) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, AttackRange[Level]);

        TowerHealthManager linkedEnemyTower = EnemyTowerHealthManager;

        foreach (var hit in hits)
        {
            SummonBase summon = hit.GetComponentInParent<SummonBase>();
            if (summon != null && summon != this && summon.IsAlive && summon.Lane == Lane && summon.IsEnemy)
            {
                summon.TakeDamage(Damage[Level]);
                continue;
            }

            TowerHealthManager tower = hit.GetComponentInParent<TowerHealthManager>();
            if (tower != null && tower.isAlive && tower.Tower != null && tower.Tower.Lane == Lane && tower.Tower.TowerTypeCheck != EnumLists.TowerType.Player)
            {
                tower.TakeDamage(Damage[Level]);
                continue;
            }

            BaseHealthManager baseHealth = hit.GetComponentInParent<BaseHealthManager>();
            if (baseHealth != null && baseHealth.isAlive && (linkedEnemyTower == null || !linkedEnemyTower.isAlive))
            {
                baseHealth.TakeDamage(Damage[Level]);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (IsValidLevel(AttackRange))
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, AttackRange[Level]);
        }
    }
}
