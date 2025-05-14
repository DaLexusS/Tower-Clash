using UnityEngine;

[CreateAssetMenu(fileName = "SummonStats", menuName = "ScriptableObjects/SummonStats", order = 1)]

public class SummonStats : ScriptableObject
{
    [SerializeField] public int Health;
    [SerializeField] public int Damage;
    [SerializeField] public float AttackRange;
    [SerializeField] public float AttackCooldown;
    [SerializeField] public float WalkSpeed;
    [SerializeField] public float PreAttackTime;

    [Range(0.01f, 5f)]
    [SerializeField] public FirstAttackDelayRange FirstAttackDelay;

    [SerializeField] public int DeathValue;
}
