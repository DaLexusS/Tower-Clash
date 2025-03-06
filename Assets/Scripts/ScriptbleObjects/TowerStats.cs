using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "ScriptableObjects/TowerStats", order = 1)]

public class TowerStats : ScriptableObject
{
    [SerializeField] public int Health;
    [SerializeField] public float Damage;
    [SerializeField] public float Range;
    [SerializeField] public float AttackCoolDown;
}
