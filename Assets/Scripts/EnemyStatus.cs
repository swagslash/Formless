using System.Linq;
using UnityEngine;

public class EnemyStatus : MonoBehaviour
{

#region ui
    public TMPro.TextMeshProUGUI HealthText;
#endregion

    private static Color GREEN = new Color(0 / 255f, 197 / 255f, 21 / 255f);
    private static Color YELLOW = new Color(230 / 255f, 145 / 255f, 56 / 255f);
    private static Color RED = new Color(255 / 255f, 66 / 255f, 0 / 255f);

    public void SetEnemyHealth(int playerHealth) {
        if (playerHealth > 3) {
            HealthText.color = GREEN;
        } else if (playerHealth > 2) {
            HealthText.color = YELLOW;
        } else {
            HealthText.color = RED;
        }

        HealthText.text = playerHealth + "HP";
    }
}