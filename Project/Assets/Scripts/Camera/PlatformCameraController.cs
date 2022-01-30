using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCameraController : MonoBehaviour
{
    public float focalDistance = 3;
    public Transform target;
    public new Transform camera;
    private Vector3 currentTargetPos;
    private Vector3 startOffset;

    public float acceleration = 10;
    public float friction = 0.1f;
    public Vector3 velocity;
    public float minMovementOffset = 2;
    public float maxMovementOffset = 10;
    public float offsetRatio = 0.5f;
    public Vector3 offset;
    private Vector3 startTargetPosition;
    
    void Start()
    {
        startTargetPosition = target.position;
        currentTargetPos = target.position;
        startOffset = transform.position - target.position;
    }

    void Update()
    {
        Vector3 targetOffset = (target.position - currentTargetPos);
        targetOffset.x *= offsetRatio;
        targetOffset.z *= offsetRatio;
        float distanceRatio = Mathf.Clamp01((targetOffset.magnitude - minMovementOffset) / (maxMovementOffset - minMovementOffset));
        velocity += targetOffset.normalized * distanceRatio * distanceRatio * acceleration * Time.deltaTime;
        velocity *= Mathf.Pow(friction, Time.deltaTime);
        currentTargetPos += velocity * Time.deltaTime;
        Vector3 direction =  (currentTargetPos - transform.position);
        direction.y = 0;
        camera.position = transform.position - focalDistance * direction.normalized + offset + Vector3.up * offsetRatio * (currentTargetPos.y - startTargetPosition.y);
        camera.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
