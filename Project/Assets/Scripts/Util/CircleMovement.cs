using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public float radius = 1;
    private float time;
    public float angularSpeed = 90;
    private Vector3 startPos;
    private Vector3 startDirection;

    void Start()
    {
        startPos = transform.position;
        startDirection = transform.right;
    }

    void Update()
    {
        time += Time.deltaTime;
        transform.position = startPos + Quaternion.AngleAxis(time * angularSpeed, Vector3.forward) * startDirection * radius;
    }
}
