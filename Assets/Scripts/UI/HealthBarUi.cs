using UnityEngine;
using UnityEngine.UI;

public class HealthBarUi : MonoBehaviour
{
    [SerializeField] Slider healthSlider;

    public void InitHealth(int Health)
    {
        healthSlider.maxValue = Health;
        healthSlider.minValue = 0;
        healthSlider.value = Health;
    }

    public void UpdateHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    public void UpgradeHealth(int maxHealth, int currentHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }
}
