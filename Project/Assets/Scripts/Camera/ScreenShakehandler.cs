using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakehandler : MonoBehaviour
{
    public float intensity = 0;
    public float maxShakeMovement = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        intensity = Player.instance.riskAnimRatio;
        float shakeIntensity = intensity * maxShakeMovement;
        transform.localPosition = Random.Range(-shakeIntensity, shakeIntensity) * Random.insideUnitCircle;
    }
}
