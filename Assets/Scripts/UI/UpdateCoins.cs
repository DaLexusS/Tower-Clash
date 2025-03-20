using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCoins : MonoBehaviour
{
    [SerializeField] TMP_Text coinsText;

    private void OnEnable()
    {
        PlayerManager.onCoinsUpdated += UpdateCoinsText;
    }

    private void OnDisable()
    {
        PlayerManager.onCoinsUpdated -= UpdateCoinsText;
    }

    public void UpdateCoinsText(int value)
    {
        string coinsToString = value.ToString();
        coinsText.text = $"Coins - {coinsToString}";
    }
}
