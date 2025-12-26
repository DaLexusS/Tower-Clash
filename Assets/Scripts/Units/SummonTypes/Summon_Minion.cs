using UnityEngine;
public class Summon_Minion : SummonBase
{
    public override void DoAttack()
    {
        Debug.Log(summonStats.DamagePerLevel[Level]);

        IDamageable victim = currentTarget.GetComponent<IDamageable>();
        victim?.TakeDamage(summonStats.DamagePerLevel[Level]);
    }
}
