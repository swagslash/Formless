using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class HuntingEnemy : MonoBehaviour
{
    private NavMeshAgent myNavMeshAgent;
    
    public GameObject target;

    public LayerMask whatIsPlayer;

    private EnemyState _state;
    
    // Patrolling
    public float patrolSpeed;
    public float wanderTimer;
    public float wanderRange;
    private float _timer;

    // Chasing
    public float chaseSpeed;
    private Vector3? _lastKnownPos;
    
    // Attacking
    public Bullet bulletPrefab;
    public Transform bulletOrigin;
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange;

    void Awake()
    {
        target = GameObject.Find("Player");
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {
        var position = transform.position;
        var playerInRange = Physics.CheckSphere(position, sightRange, whatIsPlayer);

        if (playerInRange)
        {
            // check if we can see the player if it is in range
            _lastKnownPos = visibleTargetPos();
            playerInSight = _lastKnownPos != null;
        }
        else
        {
            // if not in sight range, also not "visible"
            playerInSight = false;
        }
        
        playerInAttackRange = Physics.CheckSphere(position, attackRange, whatIsPlayer);
        if (playerInAttackRange && playerInSight)
        {
            _state = EnemyState.ATTACK;
        }
        else if (playerInSight)
        {
            _state = EnemyState.CHASE;
        }
        else
        {
            _state = EnemyState.PATROL;
        }
        Act();
    }

    private void Act()
    {
        switch (_state)
        {
            case EnemyState.PATROL:
                Patrol();
                break;
            case EnemyState.CHASE:
                ChaseLastKnownPos();
                break;
            case EnemyState.ATTACK:
                AttackTarget();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChaseLastKnownPos()
    {
        Debug.Log("Chasing");
        myNavMeshAgent.speed = chaseSpeed;
        if (_lastKnownPos != null) myNavMeshAgent.SetDestination(_lastKnownPos.Value);
        if (myNavMeshAgent.remainingDistance < 1)
        {
            _lastKnownPos = null;
        }
    }

    private void AttackTarget()
    {
        var position = transform.position;
        var targetPos = target.transform.position;
        // this may snap into place, but idgaf
        transform.LookAt(new Vector3(targetPos.x, position.y, targetPos.z));
        myNavMeshAgent.SetDestination(position);
        Debug.Log("Attack!");
        Attack(target);
    }

    private void Attack(GameObject target)
    {
        if (alreadyAttacked) {
            return;
        }

        alreadyAttacked = true;

        var bulletOriginPosition = bulletOrigin.position;
        var dirToTarget = (target.transform.position - bulletOriginPosition).normalized;
        
        var bullet = Instantiate(
            bulletPrefab,
            bulletOriginPosition,
            Quaternion.LookRotation(dirToTarget)
        );
        bullet.BulletSpeed = 5;
        bullet.Direction = dirToTarget;
        
        StartCoroutine(ResetWeaponFire());
    }
    
    IEnumerator ResetWeaponFire() {
        yield return new WaitForSeconds(5);
        alreadyAttacked = false;
    }

    private void Patrol()
    {
        _timer += Time.deltaTime;

        myNavMeshAgent.speed = patrolSpeed;

        var arrived = myNavMeshAgent.remainingDistance < 1f;
        var timedOut = _timer >= wanderTimer;
        
        // TODO decide if we want to always wander after arriving?
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

    private Vector3? visibleTargetPos()
    {
        var dirToTarget = (target.transform.position - transform.position).normalized;
        // Does the ray intersect any objects?
        if (Physics.Raycast(transform.position, dirToTarget, out var hit, sightRange))
        {
            if (hit.transform.name == "Player")
            {
                Debug.Log("Player visible");
                Debug.DrawRay(transform.position, dirToTarget * hit.distance, Color.yellow);
                return hit.transform.position;
            }
        }
        else
        {
            Debug.DrawRay(transform.position, dirToTarget * 1000, Color.white);
        }

        return null;
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
