using UnityEngine;

public class DeadPlateformActivator : MonoBehaviour
{
    [SerializeField]
    private Collider collider;

    [SerializeField]
    private Renderer renderer;

    // Start is called before the first frame update
    public void Awake()
    {
        if(collider == null) collider = GetComponent<Collider>();
        if(renderer == null) renderer = GetComponent<Renderer>();
        collider.enabled = false;
        renderer.enabled = false;
        LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnDestroy()
    {
        LivingStateManager.UnRegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnLifeStateChanges(bool isLiving)
    {
        collider.enabled = !isLiving;
        renderer.enabled = !isLiving;
    }
}

