using System;
using UnityEngine;
using UnityEngine.Events;

public class TowerHealthManager : MonoBehaviour, IDamageable
{
    public static UnityAction<BaseTower> onTowerDied;
    [SerializeField] public TowerStats TowerStats;
    [SerializeField] public BaseTower Tower;
    [SerializeField] public GameObject hitPoint;
    [SerializeField] public bool isAlive = true;
    [SerializeField] HealthBarUi healthBarUi;

    [SerializeField] GameObject towerVisual;
    [SerializeField] GameObject towerCollider;
    [SerializeField] GameObject StatCanvas;

    public int MaxHealth;
    public int Health;
    private void Awake()
    {
        MaxHealth = TowerStats.Health;
        Health = TowerStats.Health;
        healthBarUi.InitHealth(MaxHealth);
    }

    public void UpgradeHealth(int newHealth)
    {
        float healthPercentage = (float)Health / MaxHealth;
        int result = Mathf.RoundToInt(healthPercentage * newHealth);
        MaxHealth = newHealth;
        Health = result;

        healthBarUi.UpgradeHealth(MaxHealth, Health);
    }

    public void TakeDamage(int damage)
    {
        Health = Mathf.Max(0, Health - damage);

        healthBarUi.UpdateHealth(Health);

        if (Health <= 0) { OnDied(); }
    }
    public void OnDied()
    {
        isAlive = false;
        onTowerDied.Invoke(Tower);
        Tower.KillTower();

        towerVisual.SetActive(false);
        towerCollider.SetActive(false);
        StatCanvas.SetActive(false);
    }
}
