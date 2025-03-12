using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MinionStats", menuName = "ScriptableObjects/MinionStats", order = 1)]
public class MinionStats : ScriptableObject
{
    [SerializeField] public int Health;
    [SerializeField] public float Damage;
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackCooldown;
    [SerializeField] public float WalkSpeed;
}
