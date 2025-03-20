using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    public static UnityAction<BaseTower, GameObject> onTowerUpgradePressed;
    [SerializeField] public BaseTower towerStats;
    [SerializeField] public Image mark;
    [SerializeField] public Image upgradePanel;
    [SerializeField] public GameObject towerVisual;
    [SerializeField] public float tapCooldown = 1f;
    private SpriteRenderer spriteRenderer;
    private float lastTap;
    private RectTransform panelRect;
    private void Awake()
    {
        spriteRenderer = towerVisual.GetComponent<SpriteRenderer>();
        panelRect = upgradePanel.GetComponent<RectTransform>();
        lastTap = Time.time;
    }

    private void Update()
    {
        CheckTapOnSprite();
    }

    private void OnEnable()
    { 
        PlayerManager.onCoinsUpdated += isUpgradeAvailable;
    }
    private void OnDisable()
    {
        PlayerManager.onCoinsUpdated -= isUpgradeAvailable;
    }

    public void isUpgradeAvailable(int amount)
    {
        mark.gameObject.SetActive(amount > towerStats.UpgradeCost);
    }

    void CheckTapOnSprite()
    {
        if (Time.time < lastTap) { return; }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

            bool tapOnTower = spriteRenderer != null && spriteRenderer.bounds.Contains(touchPosition);
            bool tapOnPanel = IsTapOnPanel(Input.GetTouch(0).position);

            if (tapOnTower)
            {
                lastTap = Time.time + tapCooldown;
                upgradePanel.gameObject.SetActive(true);
            }
            else if (!tapOnPanel)
            {
                upgradePanel.gameObject.SetActive(false);
            }
        }
    }

    bool IsTapOnPanel(Vector2 screenPosition)
    {
        if (upgradePanel.gameObject.activeSelf)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(panelRect, screenPosition, Camera.main);
        }
        return false;
    }

    public void onUpgradePressed()
    {
        onTowerUpgradePressed.Invoke(towerStats, upgradePanel.gameObject);
    }
}
