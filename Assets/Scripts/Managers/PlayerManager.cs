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
        SummonBase.RewardOnSummonDeath += GiveCoins;
        TowerUI.onTowerUpgradePressed += onUpgradePressed;
        TimerManager.OnRewardPerSecond += GiveCoins;
        currentCoins = playerData.Coins;
    }

    private void Start()
    {
        onCoinsUpdated.Invoke(currentCoins);
    }

    private void OnDestroy()
    {
        SummonBase.RewardOnSummonDeath -= GiveCoins;
        TimerManager.OnRewardPerSecond -= GiveCoins;
        TowerUI.onTowerUpgradePressed -= onUpgradePressed;
    }
    public void GiveCoins(int amount)
    {
        currentCoins += Mathf.Abs(amount);

        onCoinsUpdated.Invoke(currentCoins);
    }

    public bool TryBuy(int price)
    {
        if (currentCoins < price)
        {
            onNoMoneyForUpgrade.Invoke();

            return false;
        }
        else
        {
            currentCoins -= price;
        }

        onCoinsUpdated.Invoke(currentCoins);

        return true;
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
