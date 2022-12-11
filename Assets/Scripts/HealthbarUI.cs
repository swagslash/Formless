using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    private int currentHealth = 0;
    private int maxHealth = 0;

    private Slider slider;
    private TextMeshProUGUI text;

    public void Awake()
    {
        slider = GetComponent<Slider>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set { currentHealth = value; UpdateUI(); }
    }

    public int MaxHealth
    {
        get => maxHealth;
        set { maxHealth = value; UpdateUI(); }
    }

    public void UpdateUI()
    {
        slider.maxValue = maxHealth;
        slider.minValue = 0;
        slider.value = currentHealth;

        text.text = currentHealth + " / " + maxHealth;
    }
}
