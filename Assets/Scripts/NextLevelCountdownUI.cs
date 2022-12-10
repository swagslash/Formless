using System.Collections;
using UnityEngine;

public class NextLevelCountdownUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CountdownTextUI;

    void Start() {
        CountdownTextUI.text = "";
    }

    public void StartCountdown() {
        StartCoroutine(StartCountDownInternal());
    }

    IEnumerator StartCountDownInternal() {
        CountdownTextUI.text = "Continuing in 5";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "Continuing in 4";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "Continuing in 3";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "Continuing in 2";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "Continuing in 1";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "";
    }
}
