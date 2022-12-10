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
        Debug.Log("OnTriggerEnter!");
        if (other.CompareTag("Player"))
        {
            // Handle bullet collision with player
            Debug.Log("Trigger Player");

            var playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.Damage(Damage);
                playerController.GetComponentInChildren<DamageIndicator>().Hit();
            }
        } 
        else if (other.CompareTag("Enemy"))
        {
            // Handle bullet collision with enemy
            var huntingEnemy = other.GetComponent<HuntingEnemy>();
            if (huntingEnemy != null) {
                huntingEnemy.Damage(Direction * -1, Damage);
                huntingEnemy.GetComponent<DamageIndicator>().Hit();
            }
        }
        Destroy(gameObject);
    }
}
