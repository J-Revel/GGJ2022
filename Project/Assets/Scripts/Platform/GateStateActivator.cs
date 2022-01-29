using UnityEngine;

public class GateStateActivator : MonoBehaviour
{
    [SerializeField]
    private Collider collider;

    // Start is called before the first frame update
    public void Awake()
    {
        if (collider == null) collider = GetComponent<Collider>();
        LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnDestroy()
    {
        LivingStateManager.UnRegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnLifeStateChanges(bool isLiving)
    {
        collider.enabled = isLiving;
    }
}

