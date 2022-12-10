using UnityEngine;

public class EnemyAlerted : MonoBehaviour
{
    public GameObject Alert;
    private Transform CameraTransform;

    void Start() {
        CameraTransform = Camera.main.transform;
    }

    void Update()
    {
        Alert.transform.LookAt(CameraTransform);
    }

    public void SetAlerted(bool alerted) {
        Alert.SetActive(alerted);
    }
}
