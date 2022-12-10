using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// Speed of bullet
    public float BulletSpeed;
    /// Direction the bullet is flying
    public Vector3 Direction;

    public float Damage;
    
    private GameManager gameManager;

    void Start() {
        Destroy(gameObject, 10);
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        transform.position += Direction * BulletSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("OnTriggerEnter");
        var livingEntity = other.GetComponent<LivingEntity>();
        if (livingEntity != null)
        {
            livingEntity.Damage(Damage);
        }
        
        if (other.CompareTag("Player")) {
            // Handle bullet collision with player
            // TODO
        } else if (other.CompareTag("Enemy")) {
            // Handle bullet collision with enemy
            // gameManager.KillEnemy(other.gameObject);
        } else {
            // Collision with some wall
            Destroy(gameObject);
        }
        Destroy(gameObject);
    }
}
