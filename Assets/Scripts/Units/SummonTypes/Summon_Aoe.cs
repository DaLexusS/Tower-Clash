using UnityEngine;

public class Summon_Aoe : SummonBase
{
    [SerializeField] public Bomb bomb;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private SpriteRenderer fakeBombSprite;

    protected override void DoAttack()
    {
        if (!IsAlive || currentTarget == null) return;

        float radius = summonStats.GetAttackRange(Level);
        int damageValue = summonStats.DamagePerLevel[Level];

        if (bomb != null)
        {
            if (fakeBombSprite != null)
                fakeBombSprite.enabled = false;

            Bomb bombClone = Instantiate(
                bomb,
                throwPoint.position,
                Quaternion.identity
            );

            bombClone.Init(
                currentTarget.transform.position,
                damageValue,
                radius,
                IsEnemy,
                Lane
            );
        }
    }

    public override void AnimEvent_AttackFinished()
    {
        if (fakeBombSprite != null)
            fakeBombSprite.enabled = true;

        base.AnimEvent_AttackFinished();
    }
}
