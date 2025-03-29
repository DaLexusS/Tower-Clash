using TMPro;
using UnityEngine;

public class UpdateStatsUi : MonoBehaviour
{
    [SerializeField] public TMP_Text level;
    [SerializeField] public TMP_Text damage;
    [SerializeField] public TMP_Text fireRate;
    [SerializeField] public TMP_Text range;

    private void OnEnable()
    {
        TowerUI.onUpdateStats += UpdateStats;
    }

    private void OnDisable()
    {
       TowerUI.onUpdateStats -= UpdateStats;
    }

    public void UpdateStats(BaseTower towerStats)
    {

        int levelIndex = towerStats.Level;
        float currentDamage = towerStats.BaseDamage[levelIndex];
        float currentFireRate = towerStats.FireRate[levelIndex];
        float currentRange = towerStats.Range[levelIndex];

        if (levelIndex >= towerStats.UpgradeCostPerLevel.Count)
        {
            level.text = $"<color=green>Max Level</color>";
            damage.text = $"<color=green>Damage : {currentDamage}</color>";
            fireRate.text = $"<color=green>Fire Rate : {currentFireRate}</color>";
            range.text = $"<color=green>Range : {currentRange}</color>";
        }
        else
        {
            level.text = $"<color=white>Level: {levelIndex}</color>";

            string damageText = $"<color=white>Damage: {currentDamage}</color>";
            if (levelIndex + 1 < towerStats.BaseDamage.Count)
            {
                float nextDamage = towerStats.BaseDamage[levelIndex + 1];
                damageText += $" <color=green>> {nextDamage}</color>";
            }
            damage.text = damageText;

            string fireRateText = $"<color=white>Fire Rate: {currentFireRate:F2}</color>";
            if (levelIndex + 1 < towerStats.FireRate.Count)
            {
                float nextFireRate = towerStats.FireRate[levelIndex + 1];
                fireRateText += $" <color=green>> {nextFireRate:F2}</color>";
            }
            fireRate.text = fireRateText;

            string rangeText = $"<color=white>Range: {currentRange:F1}</color>";
            if (levelIndex + 1 < towerStats.Range.Count)
            {
                float nextRange = towerStats.Range[levelIndex + 1];
                rangeText += $" <color=green>> {nextRange:F1}</color>";
            }

            range.text = rangeText;
        }
    }
}
