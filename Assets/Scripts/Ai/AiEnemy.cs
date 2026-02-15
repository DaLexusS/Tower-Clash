using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AiEnemy : MonoBehaviour
{
    public static UnityAction<BaseTower, BaseTower> onEnemySummons;
    [SerializeField] float decisionTime = 7f;

    [SerializeField] int decideChance = 50;
    [SerializeField] int summonOrUpgradeChance = 50;

    //TODO serlizedfield + first dicisiton delay time

    private List<BaseTower> Towers;

    public int AiMoney = 15;

    private float lastDecisionTick = -Mathf.Infinity;
    private bool loaded = false;

    private void OnEnable()
    {
        SummonBase.RewardOnSummonDeathEnemy += GainMoney;
    }
    private void OnDisable()
    {
        SummonBase.RewardOnSummonDeathEnemy -= GainMoney;
    }
    private void Update()
    {
        TryDecide();
    }

    public void InitTower(BaseTower tower)
    {
        if (Towers == null)
            Towers = new List<BaseTower>();

        Towers.Add(tower);

        loaded = true;
    }

    private void TryDecide()
    {
        if (!loaded) { return; }
        if (!CanDecide()) { return; }

        lastDecisionTick = Time.time;

        if (!Util.RollChance(decideChance)) { return;}

        if (Util.RollChance(summonOrUpgradeChance))
        {
            SummonMinion();
        }
        else
        {
            UpgradeRandomTower();
        }
    }

    private void SummonMinion()
    {
        if (Towers == null || Towers.Count == 0)
        {
            Debug.LogWarning("No towers available to upgrade.");
            return;
        }

        int randomIndex = Random.Range(0, Towers.Count - 1);
        int randomPointIndex = Random.Range(0, Towers.Count - 1);
        BaseTower chosenTower = Towers[randomIndex];
        BaseTower chosenTowerPoint = Towers[randomIndex];

        if (AiMoney < chosenTower.SummonPrice) { return; }

        WasteMoney(chosenTower.SummonPrice);

        onEnemySummons?.Invoke(chosenTower, chosenTowerPoint);
    }

    private void UpgradeRandomTower()
    {
        if (Towers == null || Towers.Count == 0)
        {
            Debug.LogWarning("No towers available to upgrade.");
            return;
        }

        int randomIndex = Random.Range(0, Towers.Count - 1);
        BaseTower chosenTower = Towers[randomIndex];

        
        if (chosenTower == null) { return; }

        int levelIndex = chosenTower.Level;

        if (!chosenTower.Alive) { return; }
        if (chosenTower.Level == chosenTower.UpgradeCostPerLevel.Count) { return; }
        if (AiMoney < chosenTower.UpgradeCostPerLevel[chosenTower.Level]) { return; }

        int towerUpgradePrice = chosenTower.UpgradeCostPerLevel[chosenTower.Level];

        WasteMoney(chosenTower.UpgradeCostPerLevel[chosenTower.Level]);

        chosenTower.Upgrade();

        Debug.Log("Upgraded Tower");
    }

    private bool CanDecide()
    {
        return Time.time >= lastDecisionTick + decisionTime;
    }

    private void WasteMoney(int amount) => AiMoney -= amount;
    private void GainMoney(int amount) => AiMoney += amount;
}
