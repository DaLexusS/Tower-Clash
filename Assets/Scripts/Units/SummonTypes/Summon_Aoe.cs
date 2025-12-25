using MoreMountains.Feedbacks;
using UnityEngine;

public class Summon_Aoe : SummonBase
{
    [SerializeField] private MMF_Player shotSFX;
    [SerializeField] private BombEffect bomb;

    public override void DoAttack()
    {
        float radius = summonStats.AttackRangePerLevel[Level];
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null || hit.gameObject == gameObject) continue;

            bool isEnemyHit = hit.CompareTag("Enemy");
            if (isEnemyHit)
            {
                damageable.TakeDamage(summonStats.DamagePerLevel[Level]);
            }
        }
        //shotSFX?.PlayFeedbacks();
        BombEffect bombClone = Instantiate(bomb, transform.position, Quaternion.identity);
        bombClone.Explode(2);
    }
}
