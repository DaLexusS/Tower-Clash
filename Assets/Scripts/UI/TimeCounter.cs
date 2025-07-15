using UnityEngine;
using TMPro;

public class TimeCounter : MonoBehaviour
{
    [SerializeField] TMP_Text timerText;

    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        int totalSeconds = Mathf.FloorToInt(elapsedTime);
        timerText.text = UiUtils.FormatTime(totalSeconds);
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResumeTimer()
    {
        isRunning = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
    }

    public int GetElapsedSeconds()
    {
        return Mathf.FloorToInt(elapsedTime);
    }
}
