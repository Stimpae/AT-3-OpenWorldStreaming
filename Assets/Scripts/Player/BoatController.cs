using System.Collections;
using System.Collections.Generic;
using Objects.Interactables;
using Player;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class BoatController : InteractableObject
{
    public static BoatController Instance { get; private set; }
    public bool m_active;

    public Camera cam;
    public NavMeshAgent boatAgent;
    public GameObject targetMarker;
    public GameObject playerMount;
    private GameObject m_tempMarker;

    [HideInInspector] public PlayerController playerRef;

    private void Awake()
    {
        cam = Camera.main;
        
        
        GameInputs inputs = new GameInputs();
        inputs.Enable();

        inputs.Player.InteractMovement.performed += e => LeftMouseButtonAction();
        inputs.Player.Attack.performed += e => Attack();
    }
    
    private void LeftMouseButtonAction()
    {
        if (m_active)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hitPoint;
            if (Physics.Raycast(ray, out hitPoint))
            {
                if (hitPoint.transform.gameObject.CompareTag("Water"))
                {
                    if (m_tempMarker)
                    {
                        Destroy(m_tempMarker);
                    }

                    m_tempMarker = Instantiate(targetMarker, hitPoint.point, Quaternion.identity);
                    SetDestination(hitPoint.point);
                }

                if (hitPoint.transform.gameObject.CompareTag("Land"))
                {
                    if (Vector3.Distance(transform.position, hitPoint.point) < 5)
                    {
                        playerRef.gameObject.transform.parent = null;
                        playerRef.gameObject.transform.position = hitPoint.point + Vector3.forward;
                        playerRef.m_active = true;
                        playerRef.player.enabled = true;
                        m_active = false;
                    }
                }
            }
        }
    }

    public void SetDestination(Vector3 destination)
    {
        boatAgent.SetDestination(destination);
    }

    private void Attack()
    {
        
    }

    public override void Interact()
    {
        Instance = this;
        if (!m_active && playerRef)
        {
            playerRef.player.enabled = false;
            playerRef.gameObject.transform.SetParent(playerMount.transform);
            playerRef.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            playerRef.m_active = false;
            m_active = true;
        }
    }
}