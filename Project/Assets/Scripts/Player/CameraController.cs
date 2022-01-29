using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float deltaAngle = 30;
    public float startAngle = 0;
    public int subdivisions = 10;
    public LayerMask layerMask;
    public bool updateMesh = false;
    public MeshFilter meshFilter;
    public float zPosition = 0;
    public Transform targetElement;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if(updateMesh)
        {
            UpdateMesh();
        }   
    }

    public void Update()
    {
        if(updateMesh)
        {
            UpdateMesh();
        }   
    }

    private Vector3 ProjectOnGamePlane(Vector3 pos)
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraDirection = cameraPosition - pos;
        float targetDistance = (pos.z - zPosition) * cameraDirection.magnitude / (pos.z - cameraPosition.z); 
        Vector3 targetPosition = pos + cameraDirection.normalized * targetDistance;
        return targetPosition;
    }

    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3 scale = transform.lossyScale;
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Vector3 startPosition = transform.position;
        Vector3 targetPosition = ProjectOnGamePlane(startPosition);
        vertices.Add(targetPosition - transform.position);
        uvs.Add(new Vector2(0, 0));

        for(int i=0; i<=subdivisions; i++)
        {
            float angle = ((float)i / subdivisions - 0.5f) * deltaAngle;
            RaycastHit hit;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * (ProjectOnGamePlane(targetElement.position) - targetPosition).normalized;
            Vector3 target = targetPosition + direction * 100;
            if(Physics.Raycast(targetPosition, direction, out hit, 100, layerMask))
            {
                target = hit.point;
            }
            vertices.Add(target - transform.position);
            float ratio = (float)i / subdivisions;
            uvs.Add(new Vector2(ratio, 1));
            if(i > 0)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i - 1);
            }
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
    }

}
