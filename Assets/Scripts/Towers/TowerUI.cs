using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour
{
    [SerializeField] private TowerHealthManager healthManager;
    public static UnityAction<BaseTower, GameObject> onTowerUpgradePressed;
    public static UnityAction onUpgradeToExpensive;

    public static TowerUI activePanel = null;

    private PlayerManager Player;
    private BaseTower towerStats = null;

    [SerializeField] public Image mark;
    [SerializeField] public Image upgradePanel;

    public RectTransform upgradePanelRect;
    public RectTransform towerUIRoot;

    [SerializeField] public GameObject towerVisual;
    [SerializeField] public float tapCooldown = 0.01f;
    [SerializeField] public float holdTimeToUpgrade = 1;
    [SerializeField] public UpdateStatsUi UpdateStats;

    private SpriteRenderer spriteRenderer;
    private RectTransform panelRect;

    private float holdTime = 0;
    private float lastTap;

    public void Init(PlayerManager player, BaseTower tower)
    {
        Player = player;
        towerStats = tower;
    }
    private void Awake()
    {
        spriteRenderer = towerVisual.GetComponent<SpriteRenderer>();
        panelRect = upgradePanel.GetComponent<RectTransform>();
        lastTap = Time.time;
        holdTime = Time.time;
    }

    private void Update()
    {
        if (!healthManager.isAlive)
        {
            mark.gameObject.SetActive(false);
            return;
        }

        if (towerStats == null)
        {
            return;
        }

        if (!towerStats.Alive) {
            mark.gameObject.SetActive(false);
            upgradePanel.gameObject.SetActive(false);
            return;
        }

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
        if (towerStats == null || towerStats.UpgradeCostPerLevel == null || towerStats.UpgradeCostPerLevel.Count == 0)
            return;

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
        if (towerStats == null || towerStats.UpgradeCostPerLevel == null || towerStats.UpgradeCostPerLevel.Count == 0)
            return true;

        return towerStats.Level >= towerStats.UpgradeCostPerLevel.Count;
    }

    public void TryUpgrade()
    {
        if(!IsMaxLevel() && holdTime >= 0.20f && Player.currentCoins < towerStats.UpgradeCostPerLevel[towerStats.Level])
                        {
            if (onUpgradeToExpensive == null) return;
            onUpgradeToExpensive.Invoke();
            return;
        }
        else
        {
            onTowerUpgradePressed.Invoke(towerStats, upgradePanel.gameObject);
        }
    }

    private void CheckTapOnSprite()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Vector2 touchPos = Input.GetTouch(0).position;

            if (TowerUI.activePanel != null)
            {
                RectTransform panel = TowerUI.activePanel.upgradePanel.GetComponent<RectTransform>();
                if (!RectTransformUtility.RectangleContainsScreenPoint(panel, touchPos, Camera.main))
                {
                    TowerUI.activePanel.upgradePanel.gameObject.SetActive(false);
                    TowerUI.activePanel = null;
                }
            }
        }
    }
    public void OpenStats()
    {
        if (activePanel != null && activePanel != this)
        {
            activePanel.upgradePanel.gameObject.SetActive(false);
        }

        bool isActive = upgradePanel.gameObject.activeSelf;

        upgradePanel.gameObject.SetActive(!isActive);

        if (!isActive)
        {
            UpdateStats.UpdateStats(towerStats);
            activePanel = this;
        }
        else
        {
            if (activePanel == this) activePanel = null;
        }
    }
}
