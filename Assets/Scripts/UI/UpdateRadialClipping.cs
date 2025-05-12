using UnityEngine;
using UnityEngine.Events;

public class UpdateRadialClipping : MonoBehaviour
{
    [SerializeField] public Material material;

    static public UnityAction<float> OnUpdateRadial;
    static public UnityAction OnResetRadial;

    private void Awake()
    {
        OnUpdateRadial += SetRadial;
        OnResetRadial += ResetRadial;
        ResetRadial();
    }

    private void OnDestroy()
    {
        OnUpdateRadial -= SetRadial;
        OnResetRadial -= ResetRadial;
    }

    private void SetRadial(float value)
    {
        material.SetFloat("_RadialClip", value);
    }

    private void ResetRadial()
    {
        material.SetFloat("_RadialClip", 360);
    }
}