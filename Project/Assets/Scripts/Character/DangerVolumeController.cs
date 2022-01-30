using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DangerVolumeController : MonoBehaviour
{
    private Volume volume;
    void Start()
    {
        volume = GetComponent<Volume>();
    }

    // Update is called once per frame
    void Update()
    {
        volume.weight = Player.instance.riskAnimRatio;
    }
}
