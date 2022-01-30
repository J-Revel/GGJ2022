using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSpawner : MonoBehaviour
{
    public AudioSource prefab;
    public AudioClip[] clips;
    public AudioMixerGroup mixerGroup;

    public bool spawnAtStart = false;
    public float minPitch = 0;
    public float maxPitch = 0;
    public float minIntensity = 1;
    public float maxIntensity = 1;

    public void PlaySound()
    {
        if(!this.isActiveAndEnabled)
            return;
        prefab.volume = Random.Range(minIntensity, maxIntensity);
        prefab.pitch = Random.Range(minPitch, maxPitch);
        prefab.clip = clips[Random.Range(0, clips.Length)];
        prefab.outputAudioMixerGroup = mixerGroup;
        AudioSource source = Instantiate(prefab, transform.position, transform.rotation);
        source.Play();
    }
}
