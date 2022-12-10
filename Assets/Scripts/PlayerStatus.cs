using System.Linq;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    public void SetCurrentBulletsInMagazine(int bulletCount) {
        var statusText = GetComponent<TMPro.TextMeshProUGUI>();
        if (bulletCount == 0) {
            statusText.text = "Reloading";
        } else if (bulletCount >= 10) {
            statusText.text = bulletCount + "x";
        } else {
            statusText.text = string.Join("", Enumerable.Repeat("|", bulletCount));
        }
    }
}