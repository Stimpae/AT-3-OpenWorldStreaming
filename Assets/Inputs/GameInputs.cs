// GENERATED AUTOMATICALLY FROM 'Assets/Inputs/GameInputs.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @GameInputs : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @GameInputs()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""GameInputs"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""92aba7ef-db25-4c68-8d81-edc4116a11f5"",
            ""actions"": [
                {
                    ""name"": ""Interact/Movement"",
                    ""type"": ""Button"",
                    ""id"": ""4d9b8f0c-b9bb-4f89-9482-e5ad448876e4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""50a91018-f166-4e53-8431-077ac66afbe7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""BoatSummon"",
                    ""type"": ""Button"",
                    ""id"": ""af55632c-5261-44da-b7f4-21cd14f47c7f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""a5a28705-128c-4933-b336-196ce989d4de"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Interact/Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8c2af3c-3f50-41a2-9cbd-0f7ee1818eb9"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": ""Press"",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f166a973-8aef-4876-a4b0-5be88ae5136d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""BoatSummon"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_InteractMovement = m_Player.FindAction("Interact/Movement", throwIfNotFound: true);
        m_Player_Attack = m_Player.FindAction("Attack", throwIfNotFound: true);
        m_Player_BoatSummon = m_Player.FindAction("BoatSummon", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_InteractMovement;
    private readonly InputAction m_Player_Attack;
    private readonly InputAction m_Player_BoatSummon;
    public struct PlayerActions
    {
        private @GameInputs m_Wrapper;
        public PlayerActions(@GameInputs wrapper) { m_Wrapper = wrapper; }
        public InputAction @InteractMovement => m_Wrapper.m_Player_InteractMovement;
        public InputAction @Attack => m_Wrapper.m_Player_Attack;
        public InputAction @BoatSummon => m_Wrapper.m_Player_BoatSummon;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @InteractMovement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteractMovement;
                @InteractMovement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteractMovement;
                @InteractMovement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnInteractMovement;
                @Attack.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnAttack;
                @BoatSummon.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBoatSummon;
                @BoatSummon.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBoatSummon;
                @BoatSummon.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnBoatSummon;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @InteractMovement.started += instance.OnInteractMovement;
                @InteractMovement.performed += instance.OnInteractMovement;
                @InteractMovement.canceled += instance.OnInteractMovement;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
                @BoatSummon.started += instance.OnBoatSummon;
                @BoatSummon.performed += instance.OnBoatSummon;
                @BoatSummon.canceled += instance.OnBoatSummon;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnInteractMovement(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
        void OnBoatSummon(InputAction.CallbackContext context);
    }
}
