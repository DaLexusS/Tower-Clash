using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpdateStatsUi : MonoBehaviour
{
    [SerializeField] Button upgradeButton;
    [SerializeField] public TMP_Text level;
    [SerializeField] public TMP_Text damage;
    [SerializeField] public TMP_Text range;
    [SerializeField] public TMP_Text health;

    [SerializeField] public TMP_Text Price;

    BaseTower _tower;

    public void UpdateStats(BaseTower towerStats)
    {
        _tower = towerStats;

        int levelIndex = towerStats.Level;
        bool isMaxLevel = levelIndex >= towerStats.UpgradeCostPerLevel.Count;

        if (isMaxLevel)
        {
            level.text = $"<color=green>Max Level</color>";
            damage.text = $"<color=green>{towerStats.BaseDamage[levelIndex]}</color>";
            range.text = $"<color=green>{towerStats.Range[levelIndex]}</color>";
            health.text = $"<color=green>{towerStats.Health[levelIndex]}</color>";
            upgradeButton.gameObject.SetActive(false);
            Price.text = "MAX";
            return;
        }

        float currentPrice = towerStats.UpgradeCostPerLevel[levelIndex];
        float currentHealth = towerStats.Health[levelIndex];
        float currentDamage = towerStats.BaseDamage[levelIndex];
        float currentRange = towerStats.Range[levelIndex];

        // Level
        string levelText = $"<color=red>{levelIndex}</color>";
        if (levelIndex + 1 < towerStats.BaseDamage.Count)
        {
            int nextLevel = levelIndex + 1;
            levelText += $" <color=green>- {nextLevel}</color>";
        }
        level.text = levelText;

        // Damage
        string damageText = $"<color=red>{currentDamage}</color>";
        if (levelIndex + 1 < towerStats.BaseDamage.Count)
        {
            float nextDamage = towerStats.BaseDamage[levelIndex + 1];
            damageText += $" <color=green>- {nextDamage}</color>";
        }
        damage.text = damageText;

        // Range
        string rangeText = $"<color=red>{currentRange:F1}</color>";
        if (levelIndex + 1 < towerStats.Range.Count)
        {
            float nextRange = towerStats.Range[levelIndex + 1];
            rangeText += $" <color=green>- {nextRange:F1}</color>";
        }
        range.text = rangeText;

        // Health
        string healthText = $"<color=red>{currentHealth}</color>";
        if (levelIndex + 1 < towerStats.Health.Count)
        {
            float nextHealth = towerStats.Health[levelIndex + 1];
            healthText += $" <color=green>- {nextHealth}</color>";
        }
        health.text = healthText;

        // Price
        Price.text = currentPrice.ToString();
        upgradeButton.gameObject.SetActive(true);
    }
}
