using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    public static UnityAction<BaseTower, GameObject> onTowerUpgradePressed;
    public static UnityAction<BaseTower> onUpdateStats;
    public static UnityAction<bool, float> onUpgradeProgress;
    public static UnityAction onUpgradeFinished;
    public static UnityAction onNoMoneyForUpgrade;

    [SerializeField] public PlayerManager player;
    [SerializeField] public BaseTower towerStats;
    [SerializeField] public Image mark;
    [SerializeField] public Image upgradePanel;
    [SerializeField] public Slider upgradeSlider;
    [SerializeField] public GameObject towerVisual;
    [SerializeField] public float tapCooldown = 0.01f;
    [SerializeField] public float holdTimeToUpgrade = 1;

    private SpriteRenderer spriteRenderer;
    private RectTransform panelRect;

    private bool inHold = false;

    private float holdTime = 0;
    private float lastTap;
    private void Awake()
    {
        spriteRenderer = towerVisual.GetComponent<SpriteRenderer>();
        panelRect = upgradePanel.GetComponent<RectTransform>();
        lastTap = Time.time;
        holdTime = Time.time;
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
        if (IsMaxLevel()) 
        { 
            mark.gameObject.SetActive(false);
            return;
        }

        if (amount >= towerStats.UpgradeCostPerLevel[towerStats.Level])
        {
            mark.gameObject.SetActive(true);
        }
        else
        {
            mark.gameObject.SetActive(false);
        }
    }

    private bool IsMaxLevel()
    {
        if (towerStats.Level >= towerStats.UpgradeCostPerLevel.Count) { return true; }
        else { return false; }
    }

    private void CheckTapOnSprite()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchWorldPos = Camera.main.ScreenToWorldPoint(touch.position);

            bool onTower = spriteRenderer != null && spriteRenderer.bounds.Contains(touchWorldPos);
            bool onPanel = IsTapOnPanel(touch.position);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (onTower)
                    {
                        inHold = true;
                        holdTime = 0;
                    }
                    break;

                case TouchPhase.Stationary:
                case TouchPhase.Moved:
                    if (inHold)
                    {
                        holdTime += Time.deltaTime;

                        if (!IsMaxLevel() && holdTime >= 0.20f && player.currentCoins < towerStats.UpgradeCostPerLevel[towerStats.Level])
                        {
                            onNoMoneyForUpgrade?.Invoke();
                            return;
                        }

                        if (holdTime >= 0.20f && !upgradeSlider.gameObject.activeSelf)
                        {
                            if (IsMaxLevel()) { return; }
                            upgradeSlider.gameObject.SetActive(true);
                            upgradeSlider.value = 0;
                        }

                        if (upgradeSlider.gameObject.activeSelf)
                        {
                            float progress = Mathf.Clamp01((holdTime - 0.25f) / holdTimeToUpgrade);
                            upgradeSlider.value = progress;

                            onUpgradeProgress?.Invoke(true, progress);

                            if (progress >= 1f)
                            {
                                inHold = false;
                                upgradeSlider.gameObject.SetActive(false);
                                onUpgradeFinished?.Invoke();
                                onTowerUpgradePressed?.Invoke(towerStats, upgradePanel.gameObject);
                                
                            }
                        }
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (inHold)
                    {
                        if (holdTime < 0.25f && onTower)
                        {
                            onUpdateStats?.Invoke(towerStats);
                            upgradePanel.gameObject.SetActive(true);
                        }

                        inHold = false;
                        holdTime = 0;
                        upgradeSlider.gameObject.SetActive(false);
                        onUpgradeProgress?.Invoke(false, 0);
                    }
                    else if (!onPanel)
                    {
                        upgradePanel.gameObject.SetActive(false);
                    }
                    break;
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
}
