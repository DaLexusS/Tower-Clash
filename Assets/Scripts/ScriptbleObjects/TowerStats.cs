using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "ScriptableObjects/TowerStats", order = 1)]

public class TowerStats : ScriptableObject
{
    [SerializeField] public string TowerName;
    [SerializeField] public int Level;
    [SerializeField] public int Health;
    [SerializeField] public float Damage;
    [SerializeField] public float FireRate;
    [SerializeField] public float Range;
    [SerializeField] public int UpgradeCost;
    [SerializeField] public float AttackCoolDown;
    [SerializeField] public float MinionSpawnTimeCooldown;
}
