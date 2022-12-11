using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{

    public void SetHealth(int currentHealth, int maxHealth)
    {
        var slider = GetComponent<Slider>();
        var text = GetComponentInChildren<TextMeshProUGUI>();

        if (slider != null && text != null)
        {
            slider.maxValue = maxHealth;
            slider.minValue = 0;
            slider.value = currentHealth;

            text.text = currentHealth + " / " + maxHealth;
        }
    }
}
