using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBombEvent : MonoBehaviour
{
    public SummonBase summon;

    public void AnimEvent_DoThrow()
    {
        if (summon != null)
            summon.AnimEvent_DoAttack();
    }
}
