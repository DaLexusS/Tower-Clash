using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FirstAttackDelayRange
{
    public float min;
    public float max;
}


[CreateAssetMenu(fileName = "MinionStats", menuName = "ScriptableObjects/MinionStats", order = 1)]

public class MinionStats : ScriptableObject
{
    [SerializeField] public int Health;
    [SerializeField] public float Damage;
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackCooldown;
    [SerializeField] public float WalkSpeed;
    [SerializeField] public float PreAttackTime;
    [SerializeField] public FirstAttackDelayRange FirstAttackDelay;
    [SerializeField] public int DeathValue;
}
