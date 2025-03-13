using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] public TowerStats TowerStats;
    [SerializeField] public bool isAlive = true;

    public int MaxHealth;
    public int Health;
    public float Damage;
    public float Range;
    public float AttackCoolDown;
    private void Awake()
    {
        MaxHealth = TowerStats.Health;
        Health = TowerStats.Health;
        Damage = TowerStats.Damage;
        Range = TowerStats.Range;
        AttackCoolDown = TowerStats.AttackCoolDown;
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
        //TEMP
        //Destroy(gameObject);
    }
}
