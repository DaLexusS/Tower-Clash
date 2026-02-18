using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumLists
{
    public enum SpawnType { Player, Enemy }
    public enum TowerType {Player, Enemy}
    public enum SpawnFormation {Single, Double, Triple, Quad}
    public enum SummonState
    {
        Idle,
        Moving,
        Attacking,
        Died
    }
}
