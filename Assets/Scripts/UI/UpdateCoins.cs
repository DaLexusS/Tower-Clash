using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateCoins : MonoBehaviour
{
    [SerializeField] TMP_Text coinsText;
    [SerializeField] TMP_Text coinsErrorText;

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
        coinsText.text = $"Coins - {coinsToString}";
    }

    public void NoMoneyEvent()
    {
        StartCoroutine(noCoinsText());
    }

    private IEnumerator noCoinsText()
    {
        coinsErrorText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        coinsErrorText.gameObject.SetActive(false);
    }
}
