using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using MoreMountains.Feedbacks;
using UnityEngine.Events;

public class SummonUi : MonoBehaviour
{
    public static UnityAction<BaseTower, BaseTower> onSummonPressed;
    public static UnityAction<BaseTower> onSummonSuccess;

    [SerializeField] public PlayerManager playerManager;
    [SerializeField] public List<SummonButtonHandler> SummonButtons;
    [SerializeField] public List<Button> LaneButtons;

    [SerializeField] public GameObject mainUi;
    [SerializeField] public MMF_Player visualPlayer;
    [SerializeField][Range(0.05f, 2f)] public float pressCooldown = 0.1f;

    private List<BaseTower> _playerTowers;

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
        }
    }

    private void TrySummon(int laneNumber, BaseTower summonTowerOwner)
    {
        int index = laneNumber - 1;
        if (index < 0 || index >= _playerTowers.Count)
        {
            Debug.Log($"Lane {laneNumber} is out of bounds! List size is {_playerTowers.Count}");
            return;
        }

        var laneTower = _playerTowers[index];

        if (playerManager.currentCoins < summonTowerOwner.SummonPrice)
        {
            Debug.Log("Not enough coins!");
            return;
        }

        if (playerManager.TryBuy(summonTowerOwner.SummonPrice))
        {
            onSummonPressed?.Invoke(summonTowerOwner, laneTower);

            onSummonSuccess?.Invoke(summonTowerOwner);
        }
    }
}
