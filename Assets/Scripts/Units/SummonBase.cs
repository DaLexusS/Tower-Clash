using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SummonBase : MonoBehaviour, IDamageable
{
    public SummonStats SummonStats { get; protected set; }

    public virtual void Init(BaseTower towerData)
    {

    }

    public virtual void TakeDamage(int amount)
    {

    }
    
    public virtual void OnDied()
    {

    }
}
