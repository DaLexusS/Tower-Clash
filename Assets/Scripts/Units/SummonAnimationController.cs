using MoreMountains.Feedbacks;
using UnityEngine;
using static EnumLists;

public class SummonAnimationController : MonoBehaviour
{
    [SerializeField] public MMF_Player DeathFeedback;

    public SummonBase summon;
    public Animator animator;


    void OnEnable()
    {
        SummonBase.OnStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        SummonBase.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(SummonBase changedSummon, SummonState state)
    {
        if (changedSummon != summon) return;

        switch (state)
        {
            case SummonState.Idle:
                animator.Play("Idle");
                break;

            case SummonState.Moving:
                animator.Play("Move");
                break;

            case SummonState.Attacking:
                animator.Play("Attack");
                break;

            case SummonState.Hit:
                animator.SetTrigger("Hit");
                break;

            case SummonState.Died:
                animator.Play("Death");
                DeathFeedback.PlayFeedbacks();
                break;
        }
    }
}
