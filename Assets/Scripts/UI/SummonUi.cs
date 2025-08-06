using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class SummonUi : MonoBehaviour
{
    public static UnityAction<BaseTower, BaseTower> onSummonPressed;

    [SerializeField] public PlayerManager playerManager;
    [SerializeField] public List<SummonButtonHandler> SummonButtons;
    [SerializeField] public List<Button> LaneButtons;

    [SerializeField] public GameObject mainUi;
    [SerializeField] public MMF_Player visualPlayer;
    [SerializeField][Range(0.05f, 2f)] public float pressCooldown = 0.1f;

    private List<BaseTower> _playerTowers;
    private bool isSelectingLane = false;
    private float lastPressTick = 0;
    private BaseTower selectedTower;

    private void OnEnable()
    {
        SummonButtonHandler.OnTrySummon += TrySummon;
    }
    private void OnDisable()
    {
        SummonButtonHandler.OnTrySummon -= TrySummon;
    }

    public void Init(List<BaseTower> playerTowers)
    {
        _playerTowers = playerTowers;

        int count = Mathf.Min(SummonButtons.Count, playerTowers.Count);

        for (int i = 0; i < count; i++)
        {
            int capturedIndex = i;

            var tower = playerTowers[capturedIndex];

            SummonButtons[capturedIndex].Init(tower);

           /* SummonButtons[capturedIndex].Init(
                tower.SummonIcon,
                tower.SummonPrice,
                () => OnSummonPressed(capturedIndex, playerTowers)
            );

*/
        }

       /* for (int i = 0; i < LaneButtons.Count; i++)
        {
            int capturedIndex = i;

            LaneButtons[capturedIndex].onClick.RemoveAllListeners();
            LaneButtons[capturedIndex].onClick.AddListener(() =>
            {
                if (!isSelectingLane) return;

                var laneTower = playerTowers[capturedIndex];
                HandleLaneSelection(laneTower);
            });
        }*/
    }

    private void TrySummon(int laneNumber, BaseTower summonTowerOwner)
    {
        var laneTower = _playerTowers[laneNumber - 1];

        if (playerManager.currentCoins < summonTowerOwner.SummonPrice)
        {
            playerManager.TryBuy(summonTowerOwner.SummonPrice);

            return;
        }

        playerManager.TryBuy(summonTowerOwner.SummonPrice);

        onSummonPressed?.Invoke(summonTowerOwner, laneTower);
    }

    private void OnSummonPressed(int index, List<BaseTower> towers)
    {
        if (!CanBePressed()) return;

        if (!isSelectingLane)
        {
            isSelectingLane = true;
            selectedTower = towers[index];
            mainUi.SetActive(true);
            visualPlayer.PlayFeedbacks();
        }
        else
        {
            isSelectingLane = false;
            mainUi.SetActive(false);
            visualPlayer.StopFeedbacks();
        }
    }
     private void HandleLaneSelection(BaseTower laneTower)
     {
         if (selectedTower == null) return;

         if (playerManager.currentCoins < selectedTower.SummonPrice)
         {
             playerManager.TryBuy(selectedTower.SummonPrice);
             return;
         }

         playerManager.TryBuy(selectedTower.SummonPrice);
         onSummonPressed?.Invoke(selectedTower, laneTower);

         isSelectingLane = false;
         mainUi.SetActive(false);
         visualPlayer.StopFeedbacks();
     }
    private bool CanBePressed()
    {
        if (Time.time < lastPressTick) return false;

        lastPressTick = Time.time + pressCooldown;
        return true;
    }
}
