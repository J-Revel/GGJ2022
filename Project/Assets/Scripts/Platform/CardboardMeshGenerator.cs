using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CardboardMeshGenerator : MonoBehaviour
{
    public float leafThickness = 0.02f;
    public float totalThickness = 0.1f;
    public float oscilationLength = 0.1f;
    public float subdivisionLength = 0.5f;
    private MeshFilter meshFilter;
    public bool updateMesh = true;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if(updateMesh)
        {
            UpdateMesh();
            updateMesh = false;
        }   
            
    }

    void Update()
    {
        if(updateMesh)
        {
            UpdateMesh();
            updateMesh = false;
        }  
    }

    public void UpdateMesh()
    {
        Mesh mesh = new Mesh();
        Vector3 scale = transform.lossyScale;
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();

        AddBoxMesh(vertices, normals, triangles, 0, leafThickness / scale.y / 2);
        AddBoxMesh(vertices, normals, triangles, 1 - leafThickness / scale.y / 2, 1);
        AddDividedBoxMesh(vertices, normals, triangles, leafThickness / scale.y, 1 - leafThickness / scale.y, leafThickness / scale.y, subdivisionLength, oscilationLength);

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();
        
        meshFilter.mesh = mesh;
    }

    public void AddQuadMesh(List<Vector3> vertices, List<Vector3> normals, List<int> triangles, Vector3 origin, Vector3 right, Vector3 up)
    {
        int startIndex = vertices.Count;
        Vector3 normal = Vector3.Cross(up, right);

        vertices.Add(origin);
        vertices.Add(origin + right);
        vertices.Add(origin + up + right);
        vertices.Add(origin + up);

        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);
        normals.Add(normal);

        triangles.Add(startIndex + 0);
        triangles.Add(startIndex + 2);
        triangles.Add(startIndex + 1);
        triangles.Add(startIndex + 0);
        triangles.Add(startIndex + 3);
        triangles.Add(startIndex + 2);
        
    }

    public void AddBoxMesh(List<Vector3> vertices, List<Vector3> normals, List<int> triangles, float yMin, float yMax)
    {
        Vector3 scale = transform.lossyScale;

        int startIndex = vertices.Count;
        AddQuadMesh(vertices, normals, triangles, new Vector3(0.5f, -0.5f + yMin, -0.5f), Vector3.left, Vector3.forward);
        AddQuadMesh(vertices, normals, triangles, new Vector3(-0.5f, -0.5f + yMax, -0.5f), Vector3.right, Vector3.forward);
        AddQuadMesh(vertices, normals, triangles, new Vector3(-0.5f, -0.5f + yMin, -0.5f), Vector3.right, Vector3.up * (yMax - yMin));
        AddQuadMesh(vertices, normals, triangles, new Vector3(0.5f, -0.5f + yMin, 0.5f), Vector3.left, Vector3.up * (yMax - yMin));
        AddQuadMesh(vertices, normals, triangles, new Vector3(-0.5f, -0.5f + yMax, -0.5f), Vector3.forward, Vector3.down * (yMax - yMin));
        AddQuadMesh(vertices, normals, triangles, new Vector3(0.5f, -0.5f + yMax, 0.5f), -Vector3.forward, Vector3.down * (yMax - yMin));
        
    }

    public void AddDividedBoxMesh(List<Vector3> vertices, List<Vector3> normals, List<int> triangles, float yMin, float yMax, float thickness, float subdivisionLength, float oscilationLength)
    {
        Vector3 scale = transform.lossyScale;
        int startIndex = vertices.Count;

        if(subdivisionLength == 0)
            return;
        for(int i=0; i<=Mathf.Ceil(scale.x / subdivisionLength); i++)
        {
            float height = (Mathf.Sin(i * subdivisionLength * 2 * Mathf.PI / oscilationLength) + 1) / 2;
            float delta = Mathf.Cos(i * subdivisionLength * 2 * Mathf.PI / oscilationLength) * subdivisionLength * 2 * Mathf.PI;
            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height - thickness / 2, -0.5f));
            normals.Add(new Vector3(delta, 1, 0).normalized);
            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height - thickness / 2, 0.5f));
            normals.Add(new Vector3(delta, 1, 0).normalized);

            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height + thickness / 2, -0.5f));
            normals.Add(new Vector3(delta, 1, 0).normalized);
            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height + thickness / 2, 0.5f));  
            normals.Add(new Vector3(delta, 1, 0).normalized);
        }

        for(int i=0; i<scale.x / subdivisionLength; i++)
        {
            triangles.Add(startIndex + i * 4);
            triangles.Add(startIndex + i * 4 + 4);
            triangles.Add(startIndex + i * 4 + 1);
            triangles.Add(startIndex + i * 4 + 4);
            triangles.Add(startIndex + i * 4 + 5);
            triangles.Add(startIndex + i * 4 + 1);
            
            triangles.Add(startIndex + i * 4 + 2);
            triangles.Add(startIndex + i * 4 + 3);
            triangles.Add(startIndex + i * 4 + 6);
            triangles.Add(startIndex + i * 4 + 6);
            triangles.Add(startIndex + i * 4 + 3);
            triangles.Add(startIndex + i * 4 + 7);

            // triangles.Add(startIndex + i * 4);
            // triangles.Add(startIndex + i * 4 + 2);
            // triangles.Add(startIndex + i * 4 + 4);
            // triangles.Add(startIndex + i * 4 + 4);
            // triangles.Add(startIndex + i * 4 + 2);
            // triangles.Add(startIndex + i * 4 + 6);

            // triangles.Add(startIndex + i * 4 + 1);
            // triangles.Add(startIndex + i * 4 + 5);
            // triangles.Add(startIndex + i * 4 + 3);
            // triangles.Add(startIndex + i * 4 + 5);
            // triangles.Add(startIndex + i * 4 + 7);
            // triangles.Add(startIndex + i * 4 + 3);
        }
        startIndex = vertices.Count;

        for(int i=0; i<=Mathf.Ceil(scale.x / subdivisionLength); i++)
        {
            float height = (Mathf.Sin(i * subdivisionLength * 2 * Mathf.PI / oscilationLength) + 1) / 2;
            float delta = Mathf.Cos(i * subdivisionLength * 2 * Mathf.PI / oscilationLength) * subdivisionLength * 2 * Mathf.PI;
            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height - thickness / 2, -0.5f));
            normals.Add(new Vector3(delta, 1, 0).normalized);
            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height - thickness / 2, 0.5f));
            normals.Add(new Vector3(delta, 1, 0).normalized);

            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height + thickness / 2, -0.5f));
            normals.Add(new Vector3(delta, 1, 0).normalized);
            vertices.Add(new Vector3(-0.5f + i / scale.x * subdivisionLength , -0.5f + yMin + (yMax - yMin) * height + thickness / 2, 0.5f));  
            normals.Add(new Vector3(delta, 1, 0).normalized);
        }
        for(int i=0; i<scale.x / subdivisionLength; i++)
        {
            // triangles.Add(startIndex + i * 4);
            // triangles.Add(startIndex + i * 4 + 4);
            // triangles.Add(startIndex + i * 4 + 1);
            // triangles.Add(startIndex + i * 4 + 4);
            // triangles.Add(startIndex + i * 4 + 5);
            // triangles.Add(startIndex + i * 4 + 1);
            
            // triangles.Add(startIndex + i * 4 + 2);
            // triangles.Add(startIndex + i * 4 + 3);
            // triangles.Add(startIndex + i * 4 + 6);
            // triangles.Add(startIndex + i * 4 + 6);
            // triangles.Add(startIndex + i * 4 + 3);
            // triangles.Add(startIndex + i * 4 + 7);

            triangles.Add(startIndex + i * 4);
            triangles.Add(startIndex + i * 4 + 2);
            triangles.Add(startIndex + i * 4 + 4);
            triangles.Add(startIndex + i * 4 + 4);
            triangles.Add(startIndex + i * 4 + 2);
            triangles.Add(startIndex + i * 4 + 6);

            triangles.Add(startIndex + i * 4 + 1);
            triangles.Add(startIndex + i * 4 + 5);
            triangles.Add(startIndex + i * 4 + 3);
            triangles.Add(startIndex + i * 4 + 5);
            triangles.Add(startIndex + i * 4 + 7);
            triangles.Add(startIndex + i * 4 + 3);
        }
    }
}
