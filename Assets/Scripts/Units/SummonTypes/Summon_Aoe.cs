using System.Collections;
using UnityEngine;
using static EnumLists;

public class Summon_Aoe : SummonBase
{
    [SerializeField] private BombEffect bomb;
    [SerializeField] public Vector3 attackOffSet;

    // Since it's a bomber maybe it should use animation event when bomb thrown
    public override IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(summonStats.PreAttackTimePerLevel[Level]);

        if (IsAlive)
        {
            SetState(SummonState.Attacking);
            DoAttack();
            lastAttackTime = Time.time + summonStats.AttackCooldownPerLevel[Level];
        }

        yield return new WaitForSeconds(1f);

        isAttacking = false;

        if (IsAlive)
        {
            FindTarget();
            SetState(SummonState.Idle);
        }
    }

    public override void DoAttack()
    {
        float radius = summonStats.GetAttackRange(Level);
        int damageValue = summonStats.DamagePerLevel[Level];

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position + attackOffSet,
            radius
        );

        if (bomb != null)
        {
            BombEffect bombClone = Instantiate(bomb, transform.position, Quaternion.identity);
            bombClone.Explode(2f);
        }

        foreach (var hit in hits)
        {
            if (IsOpponent(hit.gameObject))
            {
                IDamageable damageable = hit.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    damageValue = summonStats.DamagePerLevel[Level];
                    damageable.TakeDamage(damageValue);
                }
            }
        }
    }
}
