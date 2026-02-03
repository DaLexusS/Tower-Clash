using MoreMountains.Feedbacks;
using System.Collections;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    [SerializeField] public MMF_Player bombEffect;

    public void Explode(float lifeTime = 2f)
    {
        if (bombEffect != null)
        {
            bombEffect.StopFeedbacks();
            bombEffect.InitialDelay = 0f;
            bombEffect.PlayFeedbacks();
        }

        Destroy(gameObject, lifeTime);
    }
}
