using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistAppearInterval : MonoBehaviour
{
    public float awayDuration = 3;
    public float presentDuration = 5;
    private float time;
    private ScientistAnimations scientist;

    void Start()
    {
        scientist = GetComponent<ScientistAnimations>();
    }

    void Update()
    {
        time += Time.deltaTime;
        float animRatio = time % (awayDuration + presentDuration);
        scientist.visible = (animRatio > awayDuration);
    }
}
