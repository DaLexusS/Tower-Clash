using UnityEngine;
public class Summon_Minion : SummonBase
{
    public override void DoAttack()
    {
        IDamageable victim = currentTarget.GetComponent<IDamageable>();
        victim?.TakeDamage(summonStats.DamagePerLevel[Level]);
    }
}
