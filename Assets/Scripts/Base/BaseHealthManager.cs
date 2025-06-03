using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class BaseHealthManager : MonoBehaviour, IDamageable
{
    public static UnityAction<bool> BaseDied;
    [SerializeField] public BaseHealthStats healthStats;
    [SerializeField] public GameObject hitPoint;
    [SerializeField] public bool isAlive = true;
    [SerializeField] public bool IsPlayer = false;
    [SerializeField] GameObject Visual;

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
        isAlive = false;
        //TEMP

        BaseDied.Invoke(IsPlayer);

        Visual.SetActive(false);
        //Destroy(gameObject);
    }
}
