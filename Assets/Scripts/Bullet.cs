using UnityEngine;

public class Bullet : MonoBehaviour
{
    /// Speed of bullet
    public float BulletSpeed;
    /// Direction the bullet is flying
    public Vector3 Direction;

    public float Damage;

    void Start() {
        Destroy(gameObject, 10);
    }

    void Update()
    {
        transform.position += Direction * BulletSpeed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle bullet collision with player
            var playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.Damage(Damage);
            }
        } 
        else if (other.CompareTag("Enemy"))
        {
            // Handle bullet collision with enemy
            var huntingEnemy = other.GetComponent<HuntingEnemy>();
            if (huntingEnemy != null) {
                huntingEnemy.Damage(Direction * -1, Damage);
            }
        }
        Destroy(gameObject);
    }
}
