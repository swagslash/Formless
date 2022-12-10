using System.Collections;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    public Material DamageMaterial;
    private Material OriginalMaterial;

    void Awake() {
        OriginalMaterial = GetComponent<MeshRenderer>().material;
    }

    public void Hit() {
        StartCoroutine(HitInternal());
    }

    IEnumerator HitInternal() {
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = DamageMaterial;
        yield return new WaitForSeconds(0.1f);
        meshRenderer.material = OriginalMaterial;
    }
}
