using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TowerHealthManager : MonoBehaviour, IDamageable
{
    public static UnityAction<BaseTower> onTowerDied;
    [SerializeField] public TowerStats TowerStats;
    [SerializeField] public BaseTower Tower;
    [SerializeField] public GameObject hitPoint;
    [SerializeField] public bool isAlive = true;

    public int MaxHealth;
    public int Health;
    private void Awake()
    {
        MaxHealth = TowerStats.Health;
        Health = TowerStats.Health;
    }

    public void TakeDamage(int damage)
    {
        Health = Mathf.Max(0, Health - damage);

        if (Health <= 0) { OnDied(); }
    }
    public void OnDied()
    {
        isAlive = false;
        gameObject.SetActive(false);
        onTowerDied.Invoke(Tower);
        //TEMP
        //Destroy(gameObject);
    }
}
