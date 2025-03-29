using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCoins : MonoBehaviour
{
    [SerializeField] TMP_Text coinsText;
    [SerializeField] MMF_Player errorTextPlayer;

    private void OnEnable()
    {
        PlayerManager.onCoinsUpdated += UpdateCoinsText;
        TowerUI.onNoMoneyForUpgrade += NoMoneyEvent;
    }

    private void OnDisable()
    {
        PlayerManager.onCoinsUpdated -= UpdateCoinsText;
        TowerUI.onNoMoneyForUpgrade -= NoMoneyEvent;
    }

    public void UpdateCoinsText(int value)
    {
        string coinsToString = value.ToString();
        coinsText.text = $"{coinsToString} G";
    }

    public void NoMoneyEvent()
    {
        errorTextPlayer.PlayFeedbacks();
    }
}
