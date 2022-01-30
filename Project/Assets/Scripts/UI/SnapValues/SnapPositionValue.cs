using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapPositionValue : MonoBehaviour
{
    public Vector3 maxOffset;
    private Vector3 startPosition;
    public float targetValue = 1;
    public float movementRange = 0.5f;
    public SnapScrollbox snapScrollbox;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float ratio = Mathf.Abs(snapScrollbox.value - targetValue) / targetValue;
        transform.position = startPosition + maxOffset * Mathf.Clamp01(ratio);
    }
}
