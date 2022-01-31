using UnityEngine;

public class MovingPlateform : MonoBehaviour
{
    private bool activated;

    private float transitionDuration = 1f;
    private float transitionValue;

    [SerializeField]
    private SwitchEvent OnSwitchStateEvent;

    [SerializeField]
    private Transform activatedTarget;

    private Vector3 startingPosition;

    // Start is called before the first frame update
    void Start()
    {
        startingPosition = this.transform.position;
        transitionValue = activated ? 1f : 0f;
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        Vector3 transitionPosition = startingPosition * (1f - transitionValue) + transitionValue * activatedTarget.position;
        this.transform.position = transitionPosition;
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

    public void SwitchState(bool activated)
    {
        this.activated = activated;
        OnSwitchStateEvent?.Invoke(activated);
    }
}
