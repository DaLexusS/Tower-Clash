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

    [SerializeField] public List<Button> SummonButtons;
    [SerializeField] public List<Button> laneButtons;

    [SerializeField] public GameObject IconPrefab;
    [SerializeField] public GameObject mainUi;

    [SerializeField] public MMF_Player visualPlayer;

    [Range(0.05f, 2f)]
    [SerializeField] public float pressCooldown = 0.1f;


    private bool Pressed = false;
    private float lastPressTick = 0;
    private int lastPressedSummon;
    public void Init(List<BaseTower> playerTowers)
    {
        for (int i = 0; i < SummonButtons.Count && i < playerTowers.Count; i++)
        {
            int capturedIndex = i;
            

            CreateIcon(SummonButtons[capturedIndex], playerTowers[capturedIndex].SummonIcon, playerTowers[capturedIndex].SummonPrice);

            SummonButtons[capturedIndex].onClick.AddListener(() =>
            {
                if (!canBePressed()) return;

                if (!Pressed)
                {
                    Pressed = true;
                    mainUi.SetActive(true);
                    visualPlayer.PlayFeedbacks();
                    lastPressedSummon = capturedIndex;
                }
                else
                {
                    Pressed = false;
                    mainUi.SetActive(false);
                    visualPlayer.StopFeedbacks();
                }
            });

            laneButtons[capturedIndex].onClick.AddListener(() =>
            {
                if (!canBePressed()) return;

                var laneTower = playerTowers[capturedIndex];
                var summonTower = playerTowers[lastPressedSummon];

                if (playerManager.currentCoins < summonTower.SummonPrice)
                {
                    playerManager.TryBuy(summonTower.SummonPrice);
                    return;
                }

                OnSummonButtonPressed(summonTower, laneTower);

                playerManager.TryBuy(summonTower.SummonPrice);
                Pressed = false;
                visualPlayer.StopFeedbacks();
                mainUi.SetActive(false);
            });
        }
    }

    private void OnSummonButtonPressed(BaseTower Summontower, BaseTower laneTower)
    {
        onSummonPressed.Invoke(Summontower, laneTower);
    }

    public void CreateIcon(Button button, Sprite iconImage, int price)
    {
        GameObject icon = Instantiate(IconPrefab, button.transform);
        icon.GetComponent<Image>().sprite = iconImage;

        TMP_Text price_text = icon.GetComponentInChildren<TMP_Text>();
        price_text.text = $"{price}G";


    }

    private bool canBePressed()
    {
        if (Time.time < lastPressTick) { return false; }
        else
        {
            lastPressTick = Time.time + pressCooldown;
            return true;
        }
    }
}
