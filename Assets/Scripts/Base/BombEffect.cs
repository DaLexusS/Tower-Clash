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
            bombEffect.PlayFeedbacks();
        }

        StartCoroutine(DestroyRoutine(lifeTime));
    }

    private IEnumerator DestroyRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}