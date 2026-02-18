using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class SummonButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static UnityAction<int, BaseTower> OnTrySummon;

    public TMP_Text Price;
    public TMP_Text CooldownText;
    public Image Icon;
    public Transform Parent;

    private BaseTower Tower = null;
    private GameObject summonClone = null;
    private Camera mainCamera;

    private Vector3 targetPosition;
    private float smoothSpeed = 20f;

    private bool isOnCooldown = false;
    private CanvasGroup canvasGroup;

    public void Init(BaseTower towerData)
    {
        Tower = towerData;
        Price.text = Util.FormatWithCommas(towerData.SummonPrice);
        Icon.sprite = towerData.SummonIcon;

        if (CooldownText != null)
            CooldownText.gameObject.SetActive(false);
    }

    void Start()
    {
        mainCamera = Camera.main;

        canvasGroup = Parent.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = Parent.gameObject.AddComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        SummonUi.onSummonSuccess += HandleSummonSuccess;
    }

    private void OnDisable()
    {
        SummonUi.onSummonSuccess -= HandleSummonSuccess;
    }

    void LateUpdate()
    {
        if (summonClone != null)
        {
            FollowTouchPosition();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Tower == null || !Tower.Alive) return;
        if (isOnCooldown) return;

        AttachIcon();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isOnCooldown)
        {
            Release();
            return;
        }

        CheckLane();
        Release();
    }

    private void CheckLane()
    {
        Vector2 worldPos2D = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos2D, Vector2.zero);

        if (hit.collider != null && hit.collider.CompareTag("WalkLane"))
        {
            string laneName = hit.collider.name;
            if (laneName.StartsWith("WalkLane"))
            {
                string numberPart = laneName.Substring("WalkLane".Length);
                if (int.TryParse(numberPart, out int laneNumber))
                {
                    OnTrySummon?.Invoke(laneNumber, Tower);
                }
            }
        }
    }

    private void AttachIcon()
    {
        if (Tower.Config?.SummonPreSummonPrefab == null) return;

        summonClone = Instantiate(Tower.Config.SummonPreSummonPrefab);

        summonClone.transform.localScale = Vector3.one;

        Vector3 initialPos = Input.mousePosition;
        initialPos.z = 10f;
        summonClone.transform.position = mainCamera.ScreenToWorldPoint(initialPos);
    }


    private void Release()
    {
        if (summonClone != null) Destroy(summonClone);
        summonClone = null;
    }

    private void FollowTouchPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        targetPosition = mainCamera.ScreenToWorldPoint(mousePos);

        summonClone.transform.position = Vector3.Lerp(
            summonClone.transform.position,
            targetPosition,
            smoothSpeed * Time.unscaledDeltaTime);
    }

    private void HandleSummonSuccess(BaseTower tower)
    {
        if (tower != Tower) return;

        StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;

        float timer = Tower.Config.SummonCooldown;

        if (CooldownText != null)
            CooldownText.gameObject.SetActive(true);

        if (canvasGroup != null)
            canvasGroup.alpha = 0.6f;

        while (timer > 0)
        {
            if (CooldownText != null)
                CooldownText.text = Mathf.CeilToInt(timer).ToString();

            timer -= Time.deltaTime;
            yield return null;
        }

        if (CooldownText != null)
            CooldownText.gameObject.SetActive(false);

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        isOnCooldown = false;
    }
}
