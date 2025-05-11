using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Tooltip("Hold duration in seconds")]
    [Range(0.3f, 5f)] public float holdDuration = 1f;

    public UnityEvent onLongPress;
    public UnityEvent<int> onProgress360;
    public UnityEvent onCanceled;

    private bool isPointerDown = false;
    private bool isLongPressed = false;
    private DateTime pressTime;

    private Button button;
    private Coroutine holdRoutine;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable)
            return;

        isPointerDown = true;
        isLongPressed = false;
        pressTime = DateTime.Now;
        holdRoutine = StartCoroutine(HoldTimer());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;

        if (!isLongPressed)
        {
            onProgress360.Invoke(0);
            onCanceled.Invoke();
        }

        if (holdRoutine != null)
            StopCoroutine(holdRoutine);
    }

    private IEnumerator HoldTimer()
    {
        while (isPointerDown && !isLongPressed)
        {
            double elapsed = (DateTime.Now - pressTime).TotalSeconds;
            float percent = Mathf.Clamp01((float)(elapsed / holdDuration));
            int progress = 360 - Mathf.FloorToInt(percent * 360f);
            onProgress360.Invoke(progress);

            Debug.Log(progress);

            if (elapsed >= holdDuration)
            {
                isLongPressed = true;
                onProgress360.Invoke(0);
                onLongPress.Invoke();
                yield break;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
