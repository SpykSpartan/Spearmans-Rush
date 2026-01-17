using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAIBase : MonoBehaviour
{
    public enum AIState { Patrol, Chase, Attack, Idle }
    protected AIState currentState;

    [Header("General Settings")]
    public Transform[] patrolPoints;
    public float chaseRange = 10f;
    public float attackRange = 1f;
    public float stopDistanceBuffer = 0.5f;
    public Transform player;

    [Header("Attack Settings")]
    public int attackDamage = 10;
    public float attackCooldown = 2f;
    protected float lastAttackTime = -Mathf.Infinity;

    [Header("Dodge Settings")]
    protected bool isDodging = false;
    protected float lastDodgeTime = -Mathf.Infinity;
    protected float nextDodgeInterval = 0f;
    public float dodgeDistance = 3f;
    public float dodgeDuration = 0.3f;

    protected NavMeshAgent agent;
    protected int patrolIndex;

    protected bool canTransition = false;
    protected bool behaviorEnabled = false;

    protected float lastStateChangeTime;
    public float stateChangeCooldown = 0.5f;

    protected List<Vector3> patrolTargets = new List<Vector3>();
    protected int currentPatrolIndex = 0;
    public float patrolRadius = 5f;
    public int patrolPointCount = 3;

    protected Vector3 formationOffset = Vector3.zero;
    protected bool holdPosition = false;


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange - stopDistanceBuffer;
        currentState = AIState.Idle;
        nextDodgeInterval = Random.Range(2f, 5f);
    }

    protected virtual void Start()
    {
        currentState = AIState.Idle;

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning($"{gameObject.name} could not find player with tag 'Player'.");
            }
        }
    }

    protected virtual void Update()
    {
        if (holdPosition) return;

        TryDodge();

        if (isDodging) return;

        switch (currentState)
        {
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
            case AIState.Idle:
                Idle();
                break;
        }

        CheckTransitions();
    }

    protected virtual void TryDodge()
    {
        if (isDodging || !behaviorEnabled) return;

        if (Time.time - lastDodgeTime >= nextDodgeInterval)
        {
            lastDodgeTime = Time.time;
            nextDodgeInterval = Random.Range(2f, 5f);
            StartCoroutine(DodgeCoroutine());
        }
    }

    protected virtual IEnumerator DodgeCoroutine()
    {
        isDodging = true;

        agent.enabled = false;

        Vector3 dodgeDirection = -transform.forward;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + dodgeDirection * dodgeDistance;

        float elapsed = 0f;

        while (elapsed < dodgeDuration)
        {
            float step = (elapsed / dodgeDuration);
            transform.position = Vector3.Lerp(startPosition, targetPosition, step);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;

        agent.enabled = true;
        agent.Warp(transform.position);

        isDodging = false;
    }

    public void SetFormationOffset(Vector3 offset)
    {
        formationOffset = offset;
        holdPosition = false;
        agent.isStopped = false;
    }

    public void SetHoldingPattern()
    {
        holdPosition = true;
        agent.ResetPath();
    }


    public void EnableTransitions(bool value)
    {
        canTransition = value;
    }

    public void SetBehaviorEnabled(bool value)
    {
        behaviorEnabled = value;
    }

    protected abstract void Patrol();

    protected virtual void Chase()
    {
        if (player == null || holdPosition) return;

        Vector3 targetPosition = player.position + formationOffset;
        agent.SetDestination(targetPosition);
    }

    protected abstract void Attack();
    protected abstract void Idle();

    protected virtual void CheckTransitions()
    {
        if (player == null || !canTransition) return;
        if (holdPosition) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (Time.time - lastStateChangeTime < stateChangeCooldown)
            return;

        if (distance <= attackRange)
        {
            if (currentState != AIState.Attack)
            {
                currentState = AIState.Attack;
                lastStateChangeTime = Time.time;
            }
        }
        else if (distance <= chaseRange)
        {
            if (currentState != AIState.Chase)
            {
                currentState = AIState.Chase;
                lastStateChangeTime = Time.time;
            }
        }
        else
        {
            if (currentState != AIState.Patrol)
            {
                currentState = AIState.Patrol;
                lastStateChangeTime = Time.time;
            }
        }
    }

    protected void GenerateRandomPatrolPoints()
    {
        patrolTargets.Clear();
        currentPatrolIndex = 0;

        for (int i = 0; i < patrolPointCount; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;

            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTargets.Add(hit.position);
            }
        }

        if (patrolTargets.Count > 0)
        {
            agent.SetDestination(patrolTargets[0]);
        }
    }
}