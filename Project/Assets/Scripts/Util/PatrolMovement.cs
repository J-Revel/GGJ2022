using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolMovement : MonoBehaviour
{
    public Transform startPoint;
    public Transform targetPoint;
    private float time;
    public float duration;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        transform.position = Vector3.Lerp(startPoint.position, targetPoint.position, (1 - Mathf.Cos(time / duration * 2 * Mathf.PI))/2);
    }
}
