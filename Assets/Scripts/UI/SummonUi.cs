using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SummonUi : MonoBehaviour
{
    public static UnityAction<GameObject, GameObject, GameObject, GameObject> onSummonPressed;
    [SerializeField] public MMF_Player visualPlayer;
    [SerializeField] public PlayerManager playerManager;
    [SerializeField] public GameObject mainUi;

    [SerializeField] public GameObject summonUnit1;
    [SerializeField] public GameObject summonUnit2;
    [SerializeField] public GameObject summonUnit3;

    [SerializeField] public GameObject Spawner1;
    [SerializeField] public GameObject Spawner2;
    [SerializeField] public GameObject Spawner3;

    [SerializeField] public GameObject Lane1;
    [SerializeField] public GameObject Lane2;
    [SerializeField] public GameObject Lane3;

    [SerializeField] public GameObject Tower1;
    [SerializeField] public GameObject Tower2;
    [SerializeField] public GameObject Tower3;

    [SerializeField] public float pressCooldown = 0.5f;

    private int testPrice = 15;
    private bool Pressed = false;
    private float lastPressTick = 0;

    private GameObject unitToSpawn;

    public void onPressed(int id)
    {
        if (!canBePressed()) {  return; }
        if (!Pressed)
        {
            mainUi.SetActive(true);
            Pressed = true;
            visualPlayer.PlayFeedbacks();

            if (id == 1)
            {
                unitToSpawn = summonUnit1;
            }
            else if (id == 2)
            {
                unitToSpawn = summonUnit2;
            }
            else
            {
                unitToSpawn = summonUnit3;
            }

        }
        else
        {
            Pressed = false;
            visualPlayer.StopFeedbacks();
            mainUi.SetActive(false);
        }
    }

    public void LanePressed(int id)
    {
        if (!canBePressed()) { return; }

        if (playerManager.currentCoins < testPrice)
        {
            playerManager.TryBuy(testPrice);
            return;
        }

        if (id == 1)
        {
            onSummonPressed.Invoke(Spawner1, unitToSpawn, Lane1, Tower1);
        }
        else if (id == 2)
        {
            onSummonPressed.Invoke(Spawner2, unitToSpawn, Lane2, Tower2);
        }
        else
        {
            onSummonPressed.Invoke(Spawner3, unitToSpawn, Lane3, Tower3);
        }

        playerManager.TryBuy(testPrice);

        Pressed = false;
        visualPlayer.StopFeedbacks();
        mainUi.SetActive(false);
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
