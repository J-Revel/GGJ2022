using UnityEngine;

public class DeadPlateformActivator : MonoBehaviour
{
    [SerializeField]
    private Collider collider;

    [SerializeField]
    private GameObject visualGameObject;

    // Start is called before the first frame update
    public void Awake()
    {
        if(collider == null) collider = GetComponent<Collider>();
        collider.enabled = false;
        visualGameObject.SetActive(false);
        LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnDestroy()
    {
        LivingStateManager.UnRegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnLifeStateChanges(bool isLiving)
    {
        collider.enabled = !isLiving;
        visualGameObject.SetActive(!isLiving);
    }
}

