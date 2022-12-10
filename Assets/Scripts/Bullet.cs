using System;
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

    private void OnCollisionEnter(Collision collision)
    {
        
        Debug.Log(collision.transform.position);
        GetComponent<Collider>().enabled = false;
         var other = collision.collider;
        if (other.CompareTag("Player"))
        {
            // Handle bullet collision with player
            Debug.Log("Trigger Player");

            var playerController = other.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                playerController.Damage(Damage);
                if (playerController.Health > 0) {
                    playerController.GetComponentInChildren<DamageIndicator>().Hit();
                }
            }
        } 
        else if (other.CompareTag("Enemy"))
        {
            // Handle bullet collision with enemy
            var huntingEnemy = other.GetComponent<HuntingEnemy>();
            if (huntingEnemy != null) {
                huntingEnemy.Damage(Direction * -1, Damage);
                if (huntingEnemy.health > 0) {
                    huntingEnemy.GetComponent<DamageIndicator>().Hit();
                }
            }
        }
        Destroy(gameObject);
    }
}
