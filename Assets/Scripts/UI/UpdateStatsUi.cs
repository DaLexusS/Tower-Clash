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
        level.text = towerStats.Level.ToString();
        damage.text = towerStats.BaseDamage[towerStats.Level].ToString();
    }
}
