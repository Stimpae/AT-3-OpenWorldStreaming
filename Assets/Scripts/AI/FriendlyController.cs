using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace AI
{
    public class FriendlyController : MonoBehaviour
    {
        [Header("AI Settings")] 
        [SerializeField] private float minMovementRange = 10.0f;
        [SerializeField] private float maxMovementRange = 70.0f;
        [SerializeField] private int minWaitTimer = 2;
        [SerializeField] private int maxWaitTimer = 8;
        [SerializeField] private float movementSpeed = 3.0f;
        
        private NavMeshAgent m_agent;
        private Vector3 m_startPosition;
        private Vector3 m_targetPosition;
        private EBehaviourStates m_states;

        private void Awake()
        {
            m_agent = GetComponent<NavMeshAgent>();
            m_states = EBehaviourStates.ROAM;
        }

        // Start is called before the first frame update
        void Start()
        {
            m_startPosition = transform.position;
            m_targetPosition = GetTargetPosition();
            m_agent.speed = movementSpeed;
        
            // start the movement for the original start position
            StartCoroutine(StartMovement());
        }

        // Update is called once per frame
        void Update()
        {
            switch (m_states)
            {
                case EBehaviourStates.IDLE:
                    m_agent.isStopped = true;
                    break;
                case EBehaviourStates.ROAM:
                    
                    // if target position still hasn't been reached in a certain amount of time
                    // then we need to look for another one
                    if (Vector3.Distance(transform.position, m_targetPosition) < 1f)
                    {
                        // reached our target position and assigned a new position
                        StartCoroutine(StartMovement());
                    }
                    break;
                case EBehaviourStates.CHASE:
                    break;
                case EBehaviourStates.ATTACK:
                    break;
                case EBehaviourStates.RETREAT:
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
            m_agent.SetDestination(m_targetPosition);
        }
        
        private Vector3 GetTargetPosition()
        {
            while (true)
            {
                Vector3 targetPos = m_startPosition +
                                    Random.insideUnitSphere.normalized * Random.Range(minMovementRange, maxMovementRange);

                if (Physics.Raycast(targetPos, -Vector3.up, out var hitPoint))
                {
                    if(hitPoint.transform.gameObject.CompareTag("Land"))
                    {
                        return new Vector3(targetPos.x, m_startPosition.y, targetPos.z);
                    }
                } 
            }
        }
    }
}
