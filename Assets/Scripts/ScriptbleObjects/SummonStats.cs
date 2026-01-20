using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SummonStats", menuName = "ScriptableObjects/SummonStats", order = 1)]
public class SummonStats : ScriptableObject
{
    [SerializeField] public int Level;
    [SerializeField] public int DeathValue;

    [SerializeField] public List<int> HealthPerLevel;
    [SerializeField] public List<int> DamagePerLevel;

    [SerializeField] public List<int> WalkSpeedPerLevel;
    [SerializeField] public List<int> AttackRangePerLevel;
    [SerializeField] public List<int> SpotRangePerLevel;

    [SerializeField] public List<float> PreAttackTimePerLevel;
    [SerializeField] public List<float> AttackCooldownPerLevel;

    [Range(0.01f, 5f)]
    [SerializeField] public FirstAttackDelayRange FirstAttackDelay;

    private const float WALK_SPEED_FACTOR = 0.0026667f;
    private const float RANGE_FACTOR = 0.0125f;

    public float GetWalkSpeed(int level)
    {
        return WalkSpeedPerLevel[level] * WALK_SPEED_FACTOR;
    }

    public float GetAttackRange(int level)
    {
        return AttackRangePerLevel[level] * RANGE_FACTOR;
    }

    public float GetSpotRange(int level)
    {
        return SpotRangePerLevel[level] * RANGE_FACTOR;
    }
}
