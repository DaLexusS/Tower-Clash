using UnityEngine;
using static EnumLists;

public class Summon_Aoe : SummonBase
{
    [SerializeField] private BombEffect bomb;
    [SerializeField] private Vector3 attackOffSet;

    protected override void DoAttack()
    {
        if (!IsAlive || currentTarget == null) return;

        float radius = summonStats.GetAttackRange(Level);
        int damageValue = summonStats.DamagePerLevel[Level];

        if (bomb != null)
        {
            BombEffect bombClone = Instantiate(bomb, transform.position + attackOffSet, Quaternion.identity);
            bombClone.Explode(2f);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + attackOffSet, radius);
        foreach (var hit in hits)
        {
            if (!IsOpponent(hit.gameObject)) continue;

            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damageValue);
            }
        }
    }

}
