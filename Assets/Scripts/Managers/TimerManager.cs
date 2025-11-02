using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TimerTrigger
{
    public float TimeThreshold;
    public UnityAction OnTrigger;
    public bool HasTriggered;

    public TimerTrigger(float threshold, UnityAction onTrigger)
    {
        TimeThreshold = threshold;
        OnTrigger = onTrigger;
        HasTriggered = false;
    }
}

public class TimerManager : MonoBehaviour
{
    public static UnityAction<int> OnRewardPerSecond;
    [SerializeField] TMP_Text timerText;

    public int RoundTime = 180;
    public int RewardPerSecond = 5;
    public int RewardPerSecondOnTimeEnded = 10;

    private float currentTime;
    private bool timerRunning = false;
    public bool IsRunning => timerRunning;
    private int timeInInt;

    private List<TimerTrigger> triggers = new List<TimerTrigger>();

    private float lastRewardTime = 0f;
    private bool inLastThirtySeconds = false;
    private bool roundEnded = false;

    void Start()
    {
        ResetTimer();
        StartTimer();

        AddTrigger(30f, () =>
        {
            inLastThirtySeconds = true;
            Debug.Log("⚠️ 30 seconds left — Reward phase active");
        });

        AddTrigger(0f, () =>
        {
            roundEnded = true;
            inLastThirtySeconds = false;
            Debug.Log("💀 Sudden Death — Final reward phase active");
        });
    }

    void Update()
    {
        HandleTime();
        HandleTriggers();
        HandleRewards();
        UpdateTimerText();
    }
    private void HandleTriggers()
    {
        foreach (var trigger in triggers)
        {
            if (!trigger.HasTriggered && currentTime <= trigger.TimeThreshold)
            {
                trigger.HasTriggered = true;
                trigger.OnTrigger?.Invoke();
            }
        }
    }

    private void HandleRewards()
    {
        if (!timerRunning && !roundEnded) return;

        if (Time.time - lastRewardTime >= 1f)
        {
            lastRewardTime = Time.time;

            if (inLastThirtySeconds && currentTime > 0)
            {
                GiveReward(RewardPerSecond);
            }
            else if (roundEnded)
            {
                GiveReward(RewardPerSecondOnTimeEnded);
            }
        }
    }

    private void GiveReward(int amount)
    {
        OnRewardPerSecond.Invoke(amount);
    }

    public void AddTrigger(float threshold, UnityAction onTrigger)
    {
        triggers.Add(new TimerTrigger(threshold, onTrigger));
    }
    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            timerText.text = Util.FormatTime(timeInInt);
        }
    }
    private void HandleTime()
    {
        if (!timerRunning) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0)
        {
            currentTime = 0;
            timerRunning = false;
        }

        timeInInt = Mathf.FloorToInt(currentTime);

        RoundTime = timeInInt;
    }
    public void StartTimer() => timerRunning = true;
    public void PauseTimer() => timerRunning = false;
    public void ResetTimer() => currentTime = RoundTime;
    public bool IsLastSeconds(float threshold) => currentTime <= threshold;
}
