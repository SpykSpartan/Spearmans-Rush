using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EliteAI : EnemyAIBase
{
    [Header("Movement Settings")]
    [SerializeField] private float AISpeed = 1.5f;

    [Header("Attack Settings")]
    public float hitboxActiveDuration = 0.5f;

    private AttackHitbox attackHitbox;

    protected override void Awake()
    {
        base.Awake();

        agent.stoppingDistance = attackRange + stopDistanceBuffer;

        attackHitbox = GetComponentInChildren<AttackHitbox>(true);
        if (attackHitbox != null)
        {
            attackHitbox.parentAI = this;
            attackHitbox.DisableHitbox();
        }
        else
        {
            Debug.LogWarning("EliteAI: No AttackHitbox found in children.");
        }
    }

    protected override void Start()
    {
        currentState = AIState.Idle;
        behaviorEnabled = false;

        StartCoroutine(EnableBehaviorAfterDelay(2f));
    }

    private IEnumerator EnableBehaviorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        behaviorEnabled = true;
        EnableTransitions(true);

        currentState = AIState.Patrol;
        agent.speed = AISpeed;

        if (patrolTargets.Count == 0)
            GenerateRandomPatrolPoints();
        else
            agent.SetDestination(patrolTargets[currentPatrolIndex]);
    }


    protected override void Patrol()
    {
        agent.speed = AISpeed;

        if (patrolTargets.Count == 0)
        {
            GenerateRandomPatrolPoints();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolTargets.Count;
            agent.SetDestination(patrolTargets[currentPatrolIndex]);

            if (currentPatrolIndex == 0)
            {
                GenerateRandomPatrolPoints();
            }
        }
    }

    protected override void Chase()
    {
        if (player == null || holdPosition) return;

        agent.speed = AISpeed;

        Vector3 targetPosition = player.position + formationOffset;
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > attackRange + stopDistanceBuffer)
        {
            if (agent.isStopped)
            {
                agent.isStopped = false;
                Debug.Log("EliteAI Chase: Agent resumed chasing");
            }

            if (!agent.SetDestination(targetPosition))
            {
                Debug.LogWarning("EliteAI Chase: Failed to set destination");
            }
        }
        else
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                Debug.Log("EliteAI Chase: Agent stopped for attack");
            }
            agent.ResetPath();
        }
    }

    protected override void Attack()
    {
        agent.isStopped = true;
        transform.LookAt(player);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            Debug.Log("EliteAI: Attacking the player!");
            lastAttackTime = Time.time;

            if (attackHitbox != null)
            {
                attackHitbox.EnableHitbox();
                StartCoroutine(DisableHitboxAfterSeconds(hitboxActiveDuration));
            }
        }
    }

    protected override void Idle()
    {
        agent.isStopped = true;
        agent.ResetPath();
    }

    private IEnumerator DisableHitboxAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (attackHitbox != null)
        {
            attackHitbox.DisableHitbox();
        }
    }

    protected override void CheckTransitions()
    {
        if (player == null || !behaviorEnabled || !canTransition || holdPosition) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (Time.time - lastStateChangeTime < stateChangeCooldown)
            return;

        if (distance <= attackRange)
        {
            if (currentState != AIState.Attack)
            {
                currentState = AIState.Attack;
                lastStateChangeTime = Time.time;
                Debug.Log("EliteAI Transition to Attack");
            }
        }
        else if (distance <= chaseRange)
        {
            if (currentState != AIState.Chase)
            {
                currentState = AIState.Chase;
                lastStateChangeTime = Time.time;
                Debug.Log("EliteAI Transition to Chase");
            }
        }
        else
        {
            if (currentState != AIState.Patrol)
            {
                currentState = AIState.Patrol;
                lastStateChangeTime = Time.time;
                Debug.Log("EliteAI Transition to Patrol");
            }
        }
    }
}