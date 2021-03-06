using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float deltaAngle = 30;
    public Vector3 startDirection { get { return (ProjectOnGamePlane(targetElement.position) - ProjectOnGamePlane(transform.position)).normalized; } }
    public int subdivisions = 10;
    public LayerMask layerMask;
    public bool updateMesh = false;
    public MeshFilter meshFilter;
    public float zPosition = 0;
    public Transform targetElement;
    public float visibility = 0;

    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock propertyBlock;
    private Player observedPlayer;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        propertyBlock = new MaterialPropertyBlock();
        meshRenderer.GetPropertyBlock(propertyBlock);
        if(updateMesh)
        {
            UpdateMesh();
        }  
        propertyBlock.SetFloat("_Visibility", visibility);
        meshRenderer.SetPropertyBlock(propertyBlock);
    }

    public void Update()
    {
        propertyBlock.SetFloat("_Visibility", visibility);
        if(updateMesh && visibility >= 1)
        {
            UpdateMesh();
        }
        meshRenderer.SetPropertyBlock(propertyBlock);
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
        propertyBlock.SetVector("_Eye_Pos", targetPosition);
        vertices.Add(targetPosition - transform.position);
        uvs.Add(new Vector2(0, 0));

        bool playerStillObserved = false;
        for(int i=0; i<=subdivisions; i++)
        {
            float angle = ((float)i / subdivisions - 0.5f) * deltaAngle;
            RaycastHit hit;
            Vector3 direction = Quaternion.AngleAxis(angle, Vector3.forward) * (ProjectOnGamePlane(targetElement.position) - targetPosition).normalized;
            Vector3 target = targetPosition + direction * 100;
            if(Physics.Raycast(targetPosition, direction, out hit, 100, layerMask))
            {
                Player player = hit.collider.GetComponent<Player>();
                if(player != null)
                {
                    player.IsObserved = true;
                    observedPlayer = player;
                    playerStillObserved = true;
                }
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
        if(observedPlayer != null && !playerStillObserved)
        {
            observedPlayer.isObserved = false;
            observedPlayer = null;
        }
        
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
    }

}
