using System.Collections;
using UnityEngine;

public class CountDownUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI CountdownTextUI;

    void Start() {
        CountdownTextUI.text = "";
    }

    public void StartCountdown() {
        StartCoroutine(StartCountDownInternal());
    }

    IEnumerator StartCountDownInternal() {
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "Ready?";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "3";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "2";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "1";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "Go!";
        yield return new WaitForSeconds(1);
        CountdownTextUI.text = "";
    }
}
