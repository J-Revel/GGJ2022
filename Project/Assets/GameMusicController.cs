using UnityEngine;
using UnityEngine.Audio;

public class GameMusicController : MonoBehaviour
{
    private static GameMusicController instance;

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


    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            LivingStateManager.RegisterForLifeStateChanges(this.OnLifeStateChanges);
            DontDestroyOnLoad(this);
        }

        //Reset instance state
        instance.isLiving = true;
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
