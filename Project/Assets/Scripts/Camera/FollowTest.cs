using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTest : MonoBehaviour
{
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private float threshold = 5f;
    [SerializeField]
    private float maxSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = this.transform.position;
        Vector3 targetPosition = this.target.transform.position;
        cameraPosition.z = 0f;
        targetPosition.z = 0f;

        Vector3 targetMovement = targetPosition - cameraPosition;
        if (targetMovement.magnitude < threshold) return;

        Vector3 movement = targetMovement.normalized * maxSpeed * Time.deltaTime;
        this.transform.position += movement;
       
    }
}
