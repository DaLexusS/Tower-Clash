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

    private bool isPressed = true;

    BaseTower Tower = null;

    GameObject summonClone = null;

    private Camera mainCamera;

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

    void Update()
    {
        FollowTouchPosition();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isPressed)
        {
            isPressed = false;

            Release();
        }

        if (Tower == null)
        {
            return;
        }

        if (!Tower.Alive)
        {
            return;
        }

        isPressed = true;

        AttachIcon();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CheckLane();

        Release();
    }

    private void CheckLane()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        Vector2 worldPos2D = new Vector2(worldPos.x, worldPos.y);

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
                    // You can pass this laneNumber to your tower or summon logic here.
                    return;
                }
            }

            Debug.LogWarning("WalkLane hit, but name format invalid: " + laneName);
        }
        else
        {
            //Cancel
        }
    }

    private void AttachIcon()
    {
        if (Tower.Config == null || Tower.Config.SummonPreSummonPrefab == null)
        {
            return;
        }

        summonClone = Instantiate(Tower.Config.SummonPreSummonPrefab, Parent);
    }

    private void Release()
    {
        if (summonClone != null)
        {
            Destroy(summonClone);
        }

        summonClone = null;
        isPressed = false;
    }

    private void FollowTouchPosition()
    {
        if (summonClone == null)
        {
            return;
        }

        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);


        summonClone.transform.position = worldPos;
    }
}
