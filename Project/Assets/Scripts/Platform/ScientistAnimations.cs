using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistAnimations : MonoBehaviour
{
    public float startDistance = 5;
    public float animDuration = 2;
    public bool visible = false;
    private float animTime = 0;
    public float lightConeAnimOffset = 0.8f;
    private Vector3 startPosition;
    public CameraController lightConeRenderer;
    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if(visible)
            animTime += Time.deltaTime;
        else animTime -= Time.deltaTime;
        animTime = Mathf.Clamp(animTime, 0, animDuration);
        transform.position = startPosition + (1 - animTime / animDuration) * Vector3.forward * startDistance;
        lightConeRenderer.visibility = Mathf.Clamp01(animTime / animDuration - lightConeAnimOffset) / (1 - lightConeAnimOffset);
    }
}
