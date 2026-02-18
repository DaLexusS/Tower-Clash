using UnityEngine;

public class CallAttackEvent : MonoBehaviour
{
    public SummonBase summon;

    public void AnimEvent_DoAttack()
    {
        if (summon != null)
            summon.AnimEvent_DoAttack();
    }

    public void AnimEvent_Finished()
    {
        if (summon != null)
            summon.AnimEvent_AttackFinished();
    }
}
