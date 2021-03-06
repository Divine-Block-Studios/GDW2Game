//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Resources/Inputs/PlatformerControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlatformerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlatformerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlatformerControls"",
    ""maps"": [
        {
            ""name"": ""PlatformerDefault"",
            ""id"": ""5b834c42-e2b6-48ab-90cb-10edfcefed28"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""7154d640-ab8b-4e85-af97-d720fc7533ad"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Dash"",
                    ""type"": ""Button"",
                    ""id"": ""e65fbec8-c127-4825-a21d-772d7506e092"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Value"",
                    ""id"": ""c3677ad0-47c0-4361-8c6b-7b98202ad251"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""dc9912c3-a9a8-432f-9154-c145739d8232"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""left"",
                    ""id"": ""2a24bb64-1667-49db-9a7a-451948915b36"",
                    ""path"": ""<Keyboard>/#(A)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""22934bb4-8668-4012-b14a-ff99b1c5bd5a"",
                    ""path"": ""<Keyboard>/#(D)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""e9294620-6603-4702-ad67-85ac3772af80"",
                    ""path"": ""<Keyboard>/#(L)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Dash"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""a4fe6346-8eac-4dbf-874f-2cc06ccb827d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f5c18198-b246-421f-a4a5-c3104efe4c58"",
                    ""path"": ""<Keyboard>/#(W)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""up"",
                    ""id"": ""119915da-58ae-4b18-8c24-5312930e63e8"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // PlatformerDefault
        m_PlatformerDefault = asset.FindActionMap("PlatformerDefault", throwIfNotFound: true);
        m_PlatformerDefault_Movement = m_PlatformerDefault.FindAction("Movement", throwIfNotFound: true);
        m_PlatformerDefault_Dash = m_PlatformerDefault.FindAction("Dash", throwIfNotFound: true);
        m_PlatformerDefault_Jump = m_PlatformerDefault.FindAction("Jump", throwIfNotFound: true);
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
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // PlatformerDefault
    private readonly InputActionMap m_PlatformerDefault;
    private IPlatformerDefaultActions m_PlatformerDefaultActionsCallbackInterface;
    private readonly InputAction m_PlatformerDefault_Movement;
    private readonly InputAction m_PlatformerDefault_Dash;
    private readonly InputAction m_PlatformerDefault_Jump;
    public struct PlatformerDefaultActions
    {
        private @PlatformerControls m_Wrapper;
        public PlatformerDefaultActions(@PlatformerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlatformerDefault_Movement;
        public InputAction @Dash => m_Wrapper.m_PlatformerDefault_Dash;
        public InputAction @Jump => m_Wrapper.m_PlatformerDefault_Jump;
        public InputActionMap Get() { return m_Wrapper.m_PlatformerDefault; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlatformerDefaultActions set) { return set.Get(); }
        public void SetCallbacks(IPlatformerDefaultActions instance)
        {
            if (m_Wrapper.m_PlatformerDefaultActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnMovement;
                @Dash.started -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnDash;
                @Dash.performed -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnDash;
                @Dash.canceled -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnDash;
                @Jump.started -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlatformerDefaultActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_PlatformerDefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Dash.started += instance.OnDash;
                @Dash.performed += instance.OnDash;
                @Dash.canceled += instance.OnDash;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public PlatformerDefaultActions @PlatformerDefault => new PlatformerDefaultActions(this);
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IPlatformerDefaultActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnDash(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}
