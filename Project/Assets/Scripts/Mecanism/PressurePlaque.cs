using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BoolEvent : UnityEvent<bool> { };

public class PressurePlaque : MonoBehaviour
{
    public enum Type { Switch, Once, Duration }

    //Intern state 
    private bool isColliding;
    private bool activated;
    private bool pressed;
    private float value;

    //Raycasting cache
    private RaycastHit hit;
    int playerLayerMask;
    bool playerHitted;

    private Collider collider;

    [SerializeField]
    private BoolEvent activatedSwitchEvent;

    [SerializeField]
    private Vector3 localStartingPosition;
    [SerializeField]
    private Vector3 localPressPosition;

    [SerializeField]
    private Type type;
    [SerializeField]
    private float downDuration = 0.2f;
    [SerializeField]
    private float upDuration = 5f;

    private void Awake()
    {
        this.playerLayerMask = 1 << LayerMask.NameToLayer("Player");
        collider = GetComponent<Collider>();
        value = 0f;
        isColliding = false;
        Adapt(value);
    }

    private void Adapt(float value)
    {
        this.transform.localPosition = localStartingPosition * (1f - value) + localPressPosition * value;
    }

    private void FixedUpdate()
    {
        if (type == Type.Once && activated)
        {
            return;
        }

        //Check Collision

        isColliding = false;
        playerHitted = false;
        Vector2 offset = collider.bounds.extents;
        List<Vector3> raycastStartingPositions = new List<Vector3>();
        raycastStartingPositions.Add(this.transform.position);
        raycastStartingPositions.Add(this.transform.position + Vector3.right * offset.x);
        raycastStartingPositions.Add(this.transform.position - Vector3.right * offset.x);

        foreach (var raycastStartingPosition in raycastStartingPositions)
        {
            playerHitted = Physics.Raycast(raycastStartingPosition, Vector3.up, out hit, 0.2f, playerLayerMask);
            Debug.DrawRay(raycastStartingPosition, Vector3.up * 0.2f, playerHitted ? Color.green : Color.red, 0.05f, false);
            if (playerHitted)
            {
                isColliding = true;
            }
        }

        //Adapt Value
        value = Mathf.Clamp((value + (isColliding ? 1f / downDuration : - 1f / upDuration) * Time.fixedDeltaTime), 0f, 1f);
        Adapt(value);

        switch (type)
        {
            case Type.Switch:
                if (value == 1f && !pressed)
                {
                    pressed = true;
                    if (activated)
                    {
                        Desactivate();
                    }
                    else
                    {
                        Activate();
                    }

                }
                else if (value == 0f)
                {
                    pressed = false;
                }
                break;
            case Type.Duration:
                if (value == 1f && !pressed)
                {
                    pressed = true;
                    Activate();
                }
                else if (value == 0f && pressed)
                {
                    pressed = false;
                    Desactivate();
                }
                break;
            case Type.Once:
                if (value == 1f)
                {
                    pressed = true;
                    Activate();
                }
                break;
        }
    }

    private void Desactivate()
    {
        activated = false;
        activatedSwitchEvent?.Invoke(false);
    }

    private void Activate()
    {
        activated = true;
        activatedSwitchEvent?.Invoke(true);
    }
}
