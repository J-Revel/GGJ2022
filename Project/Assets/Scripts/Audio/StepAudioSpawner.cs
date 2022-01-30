using UnityEngine;

public class StepAudioSpawner : MonoBehaviour
{
    private AudioSpawner audioSpawner;
    public float minStepInterval = 1;
    public float maxStepInterval = 1;
    private float currentInterval;
    public bool active = false;
    private float maxIntensity;
    private float volumeRatio = 1f;

    void Start()
    {
        currentInterval = Random.Range(minStepInterval, maxStepInterval);
        audioSpawner = GetComponent<AudioSpawner>();
        maxIntensity = audioSpawner.maxIntensity;
    }

    void Update()
    {
        currentInterval -= Time.deltaTime;
        if(currentInterval < 0 && active)
        {
            audioSpawner.maxIntensity = audioSpawner.minIntensity + (maxIntensity - audioSpawner.minIntensity) * volumeRatio;
            audioSpawner.PlaySound();
            currentInterval += Random.Range(minStepInterval, maxStepInterval);
        }
    }

    public void SetVolumeIntensity(float value)
    {
        active = true;
        volumeRatio = value;
        Debug.Log("VOLUME " + volumeRatio);
    }
}
