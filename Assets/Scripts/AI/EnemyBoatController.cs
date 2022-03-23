using System;
using System.Collections;
using Player;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI
{
    public class EnemyBoatController : MonoBehaviour
    {
        [Header("AI Settings")] 
        [SerializeField] private float minMovementRange = 10.0f;
        [SerializeField] private float maxMovementRange = 40.0f;
        [SerializeField] private int minWaitTimer = 2;
        [SerializeField] private int maxWaitTimer = 8;
        [SerializeField] private float movementSpeed = 3.0f;

        [Header("Enemy Settings")] 
        [SerializeField] private float targetChaseRange = 20.0f;
        [SerializeField] private float attackRange = 2.0f;
        [SerializeField] private float attackDelay = 5.0f;
        [SerializeField] private float targetStopChaseRange = 20.0f;

        private NavMeshAgent m_enemyAgent;
        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private EBehaviourStates m_states;

        private void Awake()
        {
            // get components
            m_enemyAgent = GetComponent<NavMeshAgent>();
            m_states = EBehaviourStates.ROAM;
        }

        private void Start()
        {
            m_startPosition = transform.position;
            m_targetPosition = GetTargetPosition();
            m_enemyAgent.speed = movementSpeed;

            // start the movement for the original start position
            StartCoroutine(StartMovement());
        }

        // Update is called once per frame
        private void Update()
        {
            switch (m_states)
            {
                case EBehaviourStates.IDLE:
                    m_enemyAgent.isStopped = true;
                    break;
                case EBehaviourStates.ROAM:
                    // if we reach our target position we need to move again
                    if (Vector3.Distance(transform.position, m_targetPosition) < 1f)
                    {
                        // reached our target position and assigned a new position
                        StartCoroutine(StartMovement());
                    }

                    FindTargetEntity();
                    break;
                case EBehaviourStates.CHASE:

                    if (PlayerController.Instance.m_active == false)
                    {
                        // stops any movement coroutines that are still playing and sets player position
                        var position = PlayerController.Instance.transform.position;

                        m_enemyAgent.SetDestination(position);
                        m_targetPosition = position;

                        // checks if we are in range to attack the target
                        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) <
                            attackRange)
                        {
                            // stops the player movement and changes to attack state
                            m_enemyAgent.isStopped = true;
                            m_states = EBehaviourStates.ATTACK;
                        }

                        // the chase target is to far away start doing something else (roam)
                        if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) >
                            targetStopChaseRange)
                        {
                            m_states = EBehaviourStates.ROAM;
                        }
                    }
                    else
                    {
                        m_targetPosition = transform.position;
                        m_states = EBehaviourStates.ROAM;
                    }


                    break;
                case EBehaviourStates.ATTACK:


                    if (Time.time > attackDelay)
                    {
                        attackDelay = Time.time + 1f; // 1 is the attack rate in this instance
                        Debug.Log("Attack");
                    }

                    if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) >
                        attackRange)
                    {
                        // restarts the enemy movement and changes the state
                        m_enemyAgent.isStopped = false;
                        m_states = EBehaviourStates.CHASE;
                    }

                    break;
                case EBehaviourStates.RETREAT:

                    // check health if its in a certain range then retreat back to roaming and keep moving?

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerator StartMovement()
        {
            // resets our target position
            m_targetPosition = Vector3.zero;

            // assigns the wait time for the coroutine
            float time = Random.Range(minWaitTimer, maxWaitTimer);
            yield return new WaitForSeconds(time);

            // get a new target position
            m_targetPosition = GetTargetPosition();
            m_enemyAgent.SetDestination(m_targetPosition);
        }

        private Vector3 GetTargetPosition()
        {
            while (true)
            {
                Vector3 targetPos = m_startPosition +
                                    Random.insideUnitSphere.normalized *
                                    Random.Range(minMovementRange, maxMovementRange);

                if (Physics.Raycast(targetPos, -Vector3.up, out var hitPoint))
                {
                    if (hitPoint.transform.gameObject.CompareTag("Water"))
                    {
                        return new Vector3(targetPos.x, m_startPosition.y, targetPos.z);
                    }
                }
            }
        }

        private void FindTargetEntity()
        {
            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < targetChaseRange)
            {
                m_states = EBehaviourStates.CHASE;
            }
        }
    }
}