using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    public static UnityAction<int> onCoinsUpdated;
    public static UnityAction onNoMoneyForUpgrade;
    [SerializeField] PlayerDataStructure playerData;

    public int currentCoins { get; set; }

    private void Awake()
    {
        // MinionBehavior.onEnemyMinionKilled += GiveCoins;
        SummonBase.RewardOnSummonDeath += GiveCoins;
        TowerUI.onTowerUpgradePressed += onUpgradePressed;
        currentCoins = playerData.Coins;
    }

    private void Start()
    {
        onCoinsUpdated.Invoke(currentCoins);
    }

    private void OnDestroy()
    {
        SummonBase.RewardOnSummonDeath -= GiveCoins;
        //MinionBehavior.onEnemyMinionKilled -= GiveCoins;
        TowerUI.onTowerUpgradePressed -= onUpgradePressed;
    }
    public void GiveCoins(int amount)
    {
        currentCoins += Mathf.Abs(amount);

        onCoinsUpdated.Invoke(currentCoins);
    }

    public void TryBuy(int price)
    {
        if (currentCoins < price)
        {
            onNoMoneyForUpgrade.Invoke();
        }
        else
        {
            currentCoins -= price;
        }

        onCoinsUpdated.Invoke(currentCoins);
    }

    public void onUpgradePressed(BaseTower tower, GameObject ui)
    {
        if (tower.Level == tower.UpgradeCostPerLevel.Count) { return; }
        if (currentCoins < tower.UpgradeCostPerLevel[tower.Level]) { return; }
        ui.SetActive(false);

        int towerUpgradePrice = tower.UpgradeCostPerLevel[tower.Level];

        TryBuy(towerUpgradePrice);

        tower.Upgrade();
    }
}
