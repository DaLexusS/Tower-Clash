using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using UnityEngine;

public class BaseHealthManager : MonoBehaviour, IDamageable
{
    [SerializeField] BaseHealthStats healthStats;

    public int Health;
    public int MaxHealth;

    private void Awake()
    {
        MaxHealth = healthStats.Health;
        Health = healthStats.Health;
    }

    public void TakeDamage(int damage)
    {
        Health = math.max(0, Health - damage);

        if (Health <= 0) { OnDied(); }
    }

    public void OnDied()
    {

    }
    //Check on zero

    //Event to ui
}
