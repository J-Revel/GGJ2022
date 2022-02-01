using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class SwitchEvent : UnityEvent<bool>
{

}

public class EndDoor : MonoBehaviour
{
    [SerializeField]
    private bool activated;

    private float transitionDuration = 0.5f;
    private float transitionValue;

    [SerializeField]
    private SwitchEvent OnSwitchStateEvent;

    private float rotationActivatedValue = 140f;

    [SerializeField]
    private Transform door;

    // Start is called before the first frame update
    void Start()
    {
        transitionValue = activated ? 1f : 0f;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        float transitionRotation = rotationActivatedValue * transitionValue;
        door.transform.localRotation = Quaternion.AngleAxis(transitionRotation, Vector3.up);
    }

    // Update is called once per frame
    void Update()
    {
        if(activated && transitionValue < 1f)
        {
            transitionValue += Time.deltaTime / transitionDuration;
            UpdateVisual();
        }
        else if (!activated && transitionValue > 0f)
        {
            transitionValue -= Time.deltaTime / transitionDuration;
            UpdateVisual();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (activated)
            {
                LevelManager.LoadNextLevel();
            }
        }
    }

    public void SwitchState(bool activated)
    {
        this.activated = activated;
        OnSwitchStateEvent?.Invoke(activated);
    }
}
