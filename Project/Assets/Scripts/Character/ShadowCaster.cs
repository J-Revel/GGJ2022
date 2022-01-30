using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCaster : MonoBehaviour
{
    public Transform instance;
    public LayerMask layerMask;
    public float offset = 0.01f;

    void Start()
    {

    }

    void Update()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerMask))
        {
            instance.position = hit.point + Vector3.up * offset;
        }
    }
}
