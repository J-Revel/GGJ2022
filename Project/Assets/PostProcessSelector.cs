using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PostProcessSelector : MonoBehaviour
{
    private bool isAlive = true;
    private float volumeValue = 1f;
    private float fadeVolumeDuration = 0.25f;

    [SerializeField]
    private Volume aliveVolume;

    [SerializeField]
    private Volume deadVolume;

    [SerializeField]
    private GameObject deadParticleContainer;

    // Start is called before the first frame update
    public void Awake()
    {
        aliveVolume.weight = 1f;
        deadVolume.weight = 0f;
        LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnDestroy()
    {
        LivingStateManager.UnRegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnLifeStateChanges(bool isAlive)
    {
        this.isAlive = isAlive;
        deadParticleContainer.SetActive(!isAlive);
    }

    private void Update()
    {
        if (isAlive)
        {
            if (volumeValue < 1f)
            {
                volumeValue = Mathf.Min(1f, volumeValue + (Time.deltaTime / fadeVolumeDuration));
            }
            else
            {
                return;
            }
        }
        else
        {
            if (volumeValue > 0f)
            {
                volumeValue = Mathf.Max(0f, volumeValue - (Time.deltaTime / fadeVolumeDuration));
            }
            else
            {
                return;
            }
        }

        this.AdaptVolumes(volumeValue);
    }

    private void AdaptVolumes(float value)
    {
        aliveVolume.weight = value;
        deadVolume.weight = 1f - value;
    }
}
