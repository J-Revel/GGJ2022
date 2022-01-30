using UnityEngine;
using UnityEngine.Audio;

public class GameMusicController : MonoBehaviour
{
    private float transitionValue = 1f;
    private bool isLiving = true;

    [SerializeField]
    private float transitionDuration = 0.5f;

    [SerializeField]
    private AudioMixer mixer;

    void Update()
    {
        if(isLiving && transitionValue < 1f)
        {
            transitionValue = Mathf.Min(transitionValue + Time.deltaTime / transitionDuration);
            UpdateAudioVolume();
        }
        else if(!isLiving && transitionValue > 0f)
        {
            transitionValue = Mathf.Max(transitionValue - Time.deltaTime / transitionDuration, 0f);
            UpdateAudioVolume();
        }
    }

    private void UpdateAudioVolume()
    {
        this.mixer.SetFloat("DeadVolume", transitionValue * -80f);
        this.mixer.SetFloat("AliveVolume", (1f - transitionValue) * -80f);
    }

    // Start is called before the first frame update
    public void Awake()
    {
        LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnDestroy()
    {
        LivingStateManager.UnRegisterForLifeStateChanges(this.OnLifeStateChanges);
    }

    private void OnLifeStateChanges(bool isLiving)
    {
        this.isLiving = isLiving;
    }
}
