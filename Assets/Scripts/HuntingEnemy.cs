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

    public bool runAndGun;

    private EnemyState _state;

    // Startled
    private float turnSpeed = 2f;
    private Vector3 turnTo;
    private Quaternion turnToQuaternion;

    // Patrolling
    public float patrolSpeed;
    public float wanderTimer;
    public float wanderRange;
    private float _timer;

    // Chasing
    public float viewAngle = 60;
    public float chaseSpeed;
    private Vector3? _lastKnownPos;

    // Attacking
    public Bullet bulletPrefab;
    public Transform bulletOrigin;
    public float timeBetweenAttacks;
    public float bulletSpeed = 5;
    public float damagePerBullet = 1;
    private bool _alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSight, playerInAttackRange;

    public float health = 10;
    private GameManager _gameManager;

    void Awake()
    {
        target = GameObject.Find("Player");
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        var position = transform.position;
        var playerInRange = Physics.CheckSphere(position, sightRange, whatIsPlayer);

        // Look for player
        if (playerInRange)
        {
            // check if we can see the player if it is in range
            var visibleTarget = visibleTargetPos();
            playerInSight = visibleTarget != null;

            if (visibleTarget != null)
            {
                _lastKnownPos = visibleTarget;
            }
        }
        else
        {
            // if not in sight range, also not "visible"
            playerInSight = false;
        }

        playerInAttackRange = Physics.CheckSphere(position, attackRange, whatIsPlayer);
        if (playerInAttackRange && playerInSight)
        {
            var currentPos = transform.position;
            var targetPos = target.transform.position;

            var currentPosNoHeight = new Vector3(currentPos.x, targetPos.y, currentPos.z);
            var targetDirection = targetPos - currentPosNoHeight;
            
            var angle = Vector3.Angle(transform.forward, targetDirection);
            if (angle < 5f)
            {
                _state = EnemyState.ATTACK;
            }
            else
            {
                turnTo = targetDirection;
                _state = EnemyState.AIMING;
            }
        }
        else if (playerInSight || _lastKnownPos != null)
        {
            // Chase if we see the player or know where it was last
            _state = EnemyState.CHASE;
        }
        else
        {
            if (_state != EnemyState.STARTLED)
            {
                _state = EnemyState.PATROL;
            }
        }

        TurnIfNeeded();
        Act();
    }

    private void TurnIfNeeded()
    {
        if (turnTo != Vector3.zero)
            turnToQuaternion = Quaternion.LookRotation(turnTo);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            turnToQuaternion,
            Time.deltaTime * turnSpeed
        );
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
            case EnemyState.AIMING:
            case EnemyState.STARTLED:
                TurnIfNeeded();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ChaseLastKnownPos()
    {
        myNavMeshAgent.speed = chaseSpeed;
        if (_lastKnownPos.HasValue) myNavMeshAgent.SetDestination(_lastKnownPos.Value);
        if (myNavMeshAgent.remainingDistance < 1)
        {
            _lastKnownPos = null;
        }
    }

    private void AttackTarget()
    {
        var position = transform.position;
        var targetPos = target.transform.position;
        var targetLookPoint = new Vector3(targetPos.x, position.y, targetPos.z);
        // this may snap into place, but idgaf
        // transform.LookAt(targetLookPoint);
        myNavMeshAgent.SetDestination(position);

        Attack(target);
        _lastKnownPos = targetPos;
        // expire timer
        _timer = wanderTimer;
    }

    private void Attack(GameObject atkTarget)
    {
        if (_alreadyAttacked)
        {
            return;
        }

        _alreadyAttacked = true;

        var bulletOriginPosition = bulletOrigin.position;
        var dirToTarget = (atkTarget.transform.position - bulletOriginPosition).normalized;

        var bullet = Instantiate(
            bulletPrefab,
            bulletOriginPosition,
            Quaternion.LookRotation(dirToTarget)
        );
        bullet.BulletSpeed = bulletSpeed;
        bullet.Damage = damagePerBullet;
        bullet.Direction = dirToTarget;

        StartCoroutine(ResetAttack());
    }

    IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        _alreadyAttacked = false;
    }

    private void Patrol()
    {
        _timer += Time.deltaTime;

        myNavMeshAgent.speed = patrolSpeed;

        var arrived = myNavMeshAgent.remainingDistance < 1f;
        var timedOut = _timer >= wanderTimer;

        // TODO decide if we want to always wander after arriving?
        if (timedOut)
        {
            var newPos = RandomNavSphere(transform.position, wanderRange, -1);
            myNavMeshAgent.SetDestination(newPos);
            _timer = 0;
        }
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float distance, int layerMask)
    {
        var randomPos = Random.insideUnitSphere * distance + origin;

        NavMesh.SamplePosition(randomPos, out var navHit, distance, layerMask);

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
                //Debug.Log("Player visible");
                Debug.DrawRay(transform.position, dirToTarget * hit.distance, Color.yellow);
                var angle = Vector3.Angle(transform.forward, dirToTarget);
                return angle < viewAngle / 2 ? hit.transform.position : null;
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
        if (_lastKnownPos.HasValue)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_lastKnownPos.Value, 2);
        }
    }

    public void FixedUpdate()
    {
        if (health < 0)
        {
            _gameManager.KillEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    public void Damage(Vector3 direction, float damage)
    {
        if (_state == EnemyState.PATROL)
        {
            _state = EnemyState.STARTLED;
            turnTo = direction;
        }

        Debug.Log("Damaged for " + damage);
        health -= damage;
    }
}