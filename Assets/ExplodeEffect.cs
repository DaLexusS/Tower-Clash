using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeEffect : MonoBehaviour
{
    [SerializeField] public Animator animator;

    public void Start()
    {
        animator.Play("Explode");
    }
}
