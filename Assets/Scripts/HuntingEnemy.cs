using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HuntingEnemy : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    
    public Transform target;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;

    // Patrolling
    public float wanderTimer;
    public float wanderRange;
    private float _timer;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public GameObject projectile;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    void Awake()
    {
        target = GameObject.Find("Player").transform;
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {
        // TODO change to visibility check
        var position = transform.position;
        playerInSightRange = Physics.CheckSphere(position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patrol();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackTarget();
    }

    private void AttackTarget()
    {
        var position = transform.position;
        // transform.LookAt(new Vector3(position.x, target.position.y, target.position.z));
        myNavMeshAgent.SetDestination(position);
        Debug.Log("Attack!");
    }

    private void ChasePlayer()
    {
        myNavMeshAgent.speed = 5;
        myNavMeshAgent.SetDestination(target.position);
    }

    private void Patrol()
    {
        _timer += Time.deltaTime;

        myNavMeshAgent.speed = 3;

        var arrived = myNavMeshAgent.remainingDistance < 1f;
        var timedOut = _timer >= wanderTimer;
        
        if (timedOut) {
            var newPos = RandomNavSphere(transform.position, wanderRange, -1);
            myNavMeshAgent.SetDestination(newPos);
            _timer = 0;
        }
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask) {
        var randomPos = Random.insideUnitSphere * distance + origin;

        NavMesh.SamplePosition (randomPos, out var navHit, distance, layerMask);
 
        return navHit.position;
    }
    
    
    /// Used for Gizmo drawing in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        var position = transform.position;
        Gizmos.DrawWireSphere(position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(position, sightRange);
    }
}
