using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static UnityAction<int> onCoinsUpdated;
    [SerializeField] PlayerDataStructure playerData;

    private int currentCoins;

    private void Awake()
    {
        MinionBehavior.onEnemyMinionKilled += GiveCoins;
        currentCoins = playerData.Coins;
    }

    private void Start()
    {
        onCoinsUpdated.Invoke(currentCoins);
    }

    private void OnDestroy()
    {
        MinionBehavior.onEnemyMinionKilled -= GiveCoins;
    }
    public void GiveCoins(int amount)
    {
        currentCoins += Mathf.Abs(amount);

        onCoinsUpdated.Invoke(currentCoins);
    }
}
