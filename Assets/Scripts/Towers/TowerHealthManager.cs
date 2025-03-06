using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] public TowerStats TowerStats;
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

    }
    public void OnDied()
    {

    }
}
