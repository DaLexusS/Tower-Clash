using TMPro;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SummonButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static UnityAction<int, BaseTower> OnTrySummon;
    public TMP_Text Price;
    public Image Icon;
    public Transform Parent;

    //private bool isPressed = false;
    private BaseTower Tower = null;
    private GameObject summonClone = null;
    private Camera mainCamera;

    private Vector3 targetPosition;
    private float smoothSpeed = 20f;

    public void Init(BaseTower towerData)
    {
        Tower = towerData;
        Price.text = Util.FormatWithCommas(towerData.SummonPrice);
        Icon.sprite = towerData.SummonIcon;
    }

    void Start()
    {
        mainCamera = Camera.main;
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

        //isPressed = true;
        AttachIcon();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
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
        summonClone = Instantiate(Tower.Config.SummonPreSummonPrefab, Parent);

        Vector3 initialPos = Input.mousePosition;
        initialPos.z = 10f;
        summonClone.transform.position = mainCamera.ScreenToWorldPoint(initialPos);
    }

    private void Release()
    {
        if (summonClone != null) Destroy(summonClone);
        summonClone = null;
        //isPressed = false;
    }

    private void FollowTouchPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
        targetPosition = mainCamera.ScreenToWorldPoint(mousePos);

        summonClone.transform.position = Vector3.Lerp(summonClone.transform.position, targetPosition, smoothSpeed * Time.unscaledDeltaTime);
    }
}