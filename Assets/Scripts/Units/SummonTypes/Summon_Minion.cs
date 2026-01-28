using UnityEngine;

public class Summon_Minion : SummonBase
{
    protected override void DoAttack()
    {
        if (!IsAlive || currentTarget == null) return;

        IDamageable victim = currentTarget.GetComponent<IDamageable>();
        if (victim != null)
        {
            victim.TakeDamage(summonStats.DamagePerLevel[Level]);
        }
    }
}
