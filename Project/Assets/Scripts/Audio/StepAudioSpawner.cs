using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepAudioSpawner : MonoBehaviour
{
    private AudioSpawner audioSpawner;
    public float minStepInterval = 1;
    public float maxStepInterval = 1;
    private float currentInterval;
    public bool active = false;

    void Start()
    {
        currentInterval = Random.Range(minStepInterval, maxStepInterval);
        audioSpawner = GetComponent<AudioSpawner>();
    }

    void Update()
    {
        currentInterval -= Time.deltaTime;
        if(currentInterval < 0 && active)
        {
            audioSpawner.PlaySound();
            currentInterval += Random.Range(minStepInterval, maxStepInterval);
        }
    }
}
