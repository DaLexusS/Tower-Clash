using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeVisual : MonoBehaviour
{
    [SerializeField] public BaseTower tower;
    [SerializeField] public SpriteRenderer towerVisual;
    [SerializeField] public MMF_Player upgradeFeedback;

    public List<Sprite> TowerStatesVisuals;

    private void OnEnable()
    {
        tower.OnUpgraded.AddListener(ChangeVisuaUpgrade);
    }

    private void OnDisable()
    {
        tower.OnUpgraded.RemoveListener(ChangeVisuaUpgrade);
    }

    private void ChangeVisuaUpgrade(int level)
    {
        if (TowerStatesVisuals.Count <= (level - 1)) { return; } 
        upgradeFeedback.PlayFeedbacks();

        towerVisual.sprite = TowerStatesVisuals[level - 1];
    }
}
