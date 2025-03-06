using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BaseStats", menuName = "ScriptableObjects/BaseHealthStats", order = 1)]
public class BaseHealthStats : ScriptableObject
{
    [SerializeField] public int Health;
}
