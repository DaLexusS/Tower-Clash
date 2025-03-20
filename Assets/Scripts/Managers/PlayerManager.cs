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
        TowerUI.onTowerUpgradePressed += onUpgradePressed;
        currentCoins = playerData.Coins;
    }

    private void Start()
    {
        onCoinsUpdated.Invoke(currentCoins);
    }

    private void OnDestroy()
    {
        MinionBehavior.onEnemyMinionKilled -= GiveCoins;
        TowerUI.onTowerUpgradePressed -= onUpgradePressed;
    }
    public void GiveCoins(int amount)
    {
        currentCoins += Mathf.Abs(amount);

        onCoinsUpdated.Invoke(currentCoins);
    }

    public void onUpgradePressed(BaseTower tower, GameObject ui)
    {
        if (currentCoins < tower.UpgradeCost) { return; }
        //Temp here, later invoke to update text stats
        ui.SetActive(false);

        currentCoins -= tower.UpgradeCost;

        onCoinsUpdated.Invoke(currentCoins);

        tower.Upgrade();
    }
}
