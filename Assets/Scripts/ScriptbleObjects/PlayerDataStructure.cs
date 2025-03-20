using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "ScriptableObjects/PlayerData", order = 2)]
public class PlayerDataStructure : ScriptableObject
{
    [SerializeField] public int Coins;
    [SerializeField] public int Level;
    [SerializeField] public int Rating;
}
