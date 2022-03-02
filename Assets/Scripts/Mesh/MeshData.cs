using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshData
{
    // store everything in 1 dimensional arrays.
    [SerializeField] public float[] vertices;
    [SerializeField] public int[] triangles;
    [SerializeField] public float[] uv;
    [SerializeField] public float[] normals;

    // take in the mesh and start constructing data fields
    public MeshData(Mesh mesh)
    {
        if (mesh.vertexCount > 0)
        {
            vertices = new float[mesh.vertexCount * 3];
            for (int i = 0; i < mesh.vertexCount; i++)
            {
                vertices[i * 3] = mesh.vertices[i].x;
                vertices[i * 3 + 1] = mesh.vertices[i].y;
                vertices[i * 3 + 2] = mesh.vertices[i].z;
            }
        }

        if (mesh.triangles.Length > 0)
        {
            triangles = new int[mesh.triangles.Length];
            for (int i = 0; i < mesh.triangles.Length; i++)
            {
                triangles[i] = mesh.triangles[i];
            }
        }
    }

    // returns the mesh data related to this data object
    public Mesh GetMesh()
    {
        Mesh mesh = new Mesh();
        
        List<Vector3> verticesList = new List<Vector3>();
        for (int i = 0; i < vertices.Length / 3; i++)
        {
            verticesList.Add(new Vector3(
                vertices[i * 3], vertices[i * 3 + 1], vertices[i * 3 + 2]
            ));
        }
        mesh.SetVertices(verticesList);

        mesh.triangles = triangles;
        
        return new Mesh();
    }
}
