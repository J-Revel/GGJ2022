using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCameraController : MonoBehaviour
{
    public float focalDistance = 3;
    public Transform target;
    public new Transform camera;
    private Vector3 currentTargetPos;

    public float acceleration = 10;
    public float friction = 0.1f;
    public Vector3 velocity;
    public float minMovementOffset = 2;
    public float maxMovementOffset = 10;
    
    void Start()
    {
        currentTargetPos = target.position;
    }

    void Update()
    {
        Vector3 targetOffset = (target.position - currentTargetPos);
        float distanceRatio = Mathf.Clamp01((targetOffset.magnitude - minMovementOffset) / (maxMovementOffset - minMovementOffset));
        velocity += targetOffset.normalized * distanceRatio * distanceRatio * acceleration * Time.deltaTime;
        velocity *= Mathf.Pow(friction, Time.deltaTime);
        currentTargetPos += velocity * Time.deltaTime;
        Vector3 direction =  (currentTargetPos - transform.position).normalized;
        camera.position = transform.position - focalDistance * direction;
        camera.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
