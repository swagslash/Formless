using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// Speed of bullet
    public float BulletSpeed;
    /// Direction the bullet is flying
    public Vector3 Direction;

    void Start() {
        Destroy(gameObject, 10);
    }

    void Update()
    {
        transform.position += Direction * BulletSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            // Handle bullet collision with player
        } else if (other.tag == "Enemy") {
            // Handle bullet collision with enemy
        }
    }
}
