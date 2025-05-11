using AllIn1SpriteShader;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class UpdateRadialClipping : MonoBehaviour
{
    [SerializeField] public Material material;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void UpdateRadial(int value)
    {
        Debug.Log(value);
        material.SetFloat("_ClipRadial", value);
    }

    public void ResetRadial()
    {
        Debug.Log(0);
        material.SetFloat("_ClipRadial", 0);
    }
}
