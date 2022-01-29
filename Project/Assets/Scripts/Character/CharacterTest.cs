using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterTest : MonoBehaviour
{
    public InputAction verticalAction;
    public InputAction horizontalAction;
    
    void Start()
    {
        verticalAction.Enable();
        horizontalAction.Enable();
    }

    void Update()
    {
        Debug.Log(verticalAction.ReadValue<float>() + " " + horizontalAction.ReadValue<float>());
    }
}
