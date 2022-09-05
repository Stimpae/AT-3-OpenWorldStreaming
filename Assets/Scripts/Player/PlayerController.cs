using System;
using System.Security.Cryptography;
using Objects.Interactables;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        public Camera cam;
        public NavMeshAgent player;
        public GameObject targetMarker;
        public GameObject boat;

        private GameObject tempMarker;
        private GameObject tempBoat;
        public bool m_active;
        private InteractableObject currentHoverTarget;

        private void Awake()
        {
            Instance = this;
            
            m_active = true;
            cam = Camera.main;
            

            GameInputs inputs = new GameInputs();
            inputs.Enable();

            inputs.Player.InteractMovement.performed += e => LeftMouseButtonAction();
            inputs.Player.Attack.performed += e => Attack();
            inputs.Player.BoatSummon.performed += e => SpawnBoat();
        }

        private void Start()
        { 
            
            
        }

        private void Update()
        {
            if (m_active)
            {
                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hitPoint;
                if (Physics.Raycast(ray, out hitPoint))
                {
                    InteractableObject targetObject = hitPoint.transform.gameObject.GetComponent<InteractableObject>();
                    if (targetObject != null)
                    {
                        if (currentHoverTarget != targetObject)
                        {
                            if (currentHoverTarget != null)
                            {
                                currentHoverTarget.UnHover();
                            }
                            
                            targetObject.Hover();
                            currentHoverTarget = targetObject;
                        }
                    }
                    else if(currentHoverTarget != null)
                    {
                        currentHoverTarget.UnHover();
                        currentHoverTarget = null;
                    }
                }
            }
        }

        private void LeftMouseButtonAction()
        {
            if (m_active)
            {
                if (currentHoverTarget != null)
                {
                    // this needs it own float for distance
                    if (Vector3.Distance(player.transform.position, currentHoverTarget.transform.position) < 5f)
                    {
                        currentHoverTarget.Interact();
                    }
                    
                }
                
                //Vector3 worldPos = Camera.main.ScreenToWorldPoint();
                Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
                RaycastHit hitPoint;
                if (Physics.Raycast(ray, out hitPoint))
                {
                    if (hitPoint.transform.gameObject.CompareTag("Land"))
                    {
                        if (tempMarker)
                        {
                            Destroy(tempMarker);
                        }

                        tempMarker = Instantiate(targetMarker, hitPoint.point, Quaternion.identity);
                        player.SetDestination(hitPoint.point);
                    }
                }
                
            }
        }

        private void Attack()
        {
            // get the current mouse position
            // rotate towards that
        }

        private void SpawnBoat()
        {
            if (m_active)
            {
                Vector3 spawnPosition = new Vector3();
                float spawnDistance = 1f;
                bool validPosition = false;
                while (true)
                {
                    Vector3 playerPos = player.transform.position;
                    Vector3 topPlayerPos = new Vector3(playerPos.x, 5, playerPos.z);
                    Vector3 playerDir = player.transform.forward;

                    Vector3 targetPos = topPlayerPos + playerDir * spawnDistance;

                    if (Physics.Raycast(targetPos, -Vector3.up, out var hitPoint))
                    {
                        if (hitPoint.transform.gameObject.CompareTag("Water"))
                        {
                            spawnPosition = new Vector3(targetPos.x, -3, targetPos.z);
                            validPosition = true;
                            break;
                        }

                        spawnDistance += 2;
                        if (spawnDistance > 10)
                        {
                            validPosition = false;
                            break;
                        }
                    }
                }
                
                if (tempBoat && validPosition)
                {
                    tempBoat.GetComponent<BoatController>().SetDestination(spawnPosition);
                }
                else if (validPosition)
                {
                    tempBoat = Instantiate(boat, spawnPosition, Quaternion.identity);
                    tempBoat.GetComponent<BoatController>().playerRef = this;
                }
            }
        }
    }
}