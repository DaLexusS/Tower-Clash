using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonStats", menuName = "ScriptableObjects/SummonStats", order = 1)]

public class SummonStats : ScriptableObject
{
    [SerializeField] public int Level;
    [SerializeField] public int DeathValue;
    [SerializeField] public List<int> HealthPerLevel;
    [SerializeField] public List<int> DamagePerLevel;
    [SerializeField] public List<float> WalkSpeedPerLevel;
    [SerializeField] public List<float> AttackRangePerLevel;
    [SerializeField] public List<float> SpotRangePerLevel;
    [SerializeField] public List<float> PreAttackTimePerLevel;
    [SerializeField] public List<float> AttackCooldownPerLevel;

    [Range(0.01f, 5f)]
    [SerializeField] public FirstAttackDelayRange FirstAttackDelay;
}
