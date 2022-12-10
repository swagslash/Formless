using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public float health = 10;
    public bool isEnemy;
    private GameManager gameManager;

    void Start() {
        gameManager = FindObjectOfType<GameManager>();
    }
    
    public void Damage(float damage)
    {
        Debug.Log("Damaged for " + damage);
        health -= damage;
    }

    public void FixedUpdate()
    {
        if (health < 0)
        {
            if (isEnemy)
            {
                //gameManager.KillEnemy(gameObject);
            }
            Destroy(gameObject);
        }
    }
}