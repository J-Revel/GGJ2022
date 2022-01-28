//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.2.0
//     from Assets/Config/Input/Player Controls.inputactions
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

public partial class @PlayerControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""Player Controls"",
    ""maps"": [
        {
            ""name"": ""Keyboard Map"",
            ""id"": ""13c2731d-ae15-4793-999c-3155f0c2527e"",
            ""actions"": [
                {
                    ""name"": ""Vertical"",
                    ""type"": ""Button"",
                    ""id"": ""ad1c5fe3-fae0-49e6-b436-39ee88f86604"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c0b1e6bf-70d2-412c-a914-642aaa393107"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e9730077-a3b0-4ab3-9e1a-c233baaea8c1"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": """",
                    ""action"": ""Vertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Keyboard Map
        m_KeyboardMap = asset.FindActionMap("Keyboard Map", throwIfNotFound: true);
        m_KeyboardMap_Vertical = m_KeyboardMap.FindAction("Vertical", throwIfNotFound: true);
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

    // Keyboard Map
    private readonly InputActionMap m_KeyboardMap;
    private IKeyboardMapActions m_KeyboardMapActionsCallbackInterface;
    private readonly InputAction m_KeyboardMap_Vertical;
    public struct KeyboardMapActions
    {
        private @PlayerControls m_Wrapper;
        public KeyboardMapActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Vertical => m_Wrapper.m_KeyboardMap_Vertical;
        public InputActionMap Get() { return m_Wrapper.m_KeyboardMap; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(KeyboardMapActions set) { return set.Get(); }
        public void SetCallbacks(IKeyboardMapActions instance)
        {
            if (m_Wrapper.m_KeyboardMapActionsCallbackInterface != null)
            {
                @Vertical.started -= m_Wrapper.m_KeyboardMapActionsCallbackInterface.OnVertical;
                @Vertical.performed -= m_Wrapper.m_KeyboardMapActionsCallbackInterface.OnVertical;
                @Vertical.canceled -= m_Wrapper.m_KeyboardMapActionsCallbackInterface.OnVertical;
            }
            m_Wrapper.m_KeyboardMapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Vertical.started += instance.OnVertical;
                @Vertical.performed += instance.OnVertical;
                @Vertical.canceled += instance.OnVertical;
            }
        }
    }
    public KeyboardMapActions @KeyboardMap => new KeyboardMapActions(this);
    public interface IKeyboardMapActions
    {
        void OnVertical(InputAction.CallbackContext context);
    }
}