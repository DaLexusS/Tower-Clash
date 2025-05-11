using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "ScriptableObjects/TowerStats", order = 1)]
public class TowerStats : ScriptableObject
{
    [Header("Basic Data")]
    [SerializeField] public string TowerName;
    [SerializeField] public int Level;
    [SerializeField] public int Health;
    [SerializeField] public float AttackCoolDown;
    [SerializeField] public float MinionSpawnTimeCooldown;
    
    [Header("Per Level Data")]
    [SerializeField] public List<int> UpgradeCostPerLevel;
    [SerializeField] public List<float> DamagePerLevel;
    [SerializeField] public List<float> FireRatePerLevel;
    [SerializeField] public List<float> RangePerLevel;

    [Header("Summon Data")]
    [SerializeField] public Sprite SummonIcon;
    [SerializeField] public int SummonPrice;
    [SerializeField] public MinionBehavior Summon;
}
