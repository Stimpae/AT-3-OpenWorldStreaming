using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class MeshData
{
    // store everything in 1 dimensional arrays.
    public float[] vertices;
    public int[] triangles;
    public float[] uv;
    public float[] normals;
    public float[] colours;

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

        if (mesh.uv.Length > 0)
        {
            uv = new float[mesh.uv.Length * 2];
            for (int i = 0; i < mesh.uv.Length; i++)
            {
                uv[i * 2] = mesh.uv[i].x;
                uv[i * 2 + 1] = mesh.uv[i].y;
            }
        }

        if (mesh.colors.Length > 0)
        {
            colours = new float[mesh.colors.Length * 4];
            for (int i = 0; i < mesh.colors.Length; i++)
            {
                colours[i * 4] = mesh.colors[i].r;
                colours[i * 4 + 1] = mesh.colors[i].g;
                colours[i * 4 + 2] = mesh.colors[i].b;
                colours[i * 4 + 3] = mesh.colors[i].a;
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
        
        List<Vector2> uvList = new List<Vector2>();
        for (int i = 0; i < uv.Length / 2; i++)
        { 
            uvList.Add(new Vector2(uv[i*2], uv[i*2+1]));
        }
        
        List<Color> colourList = new List<Color>();
        if (colours != null)
        {
            for (int i = 0; i < colours.Length / 4; i++)
            {
                colourList.Add(new Color(colours[i * 4],colours[i * 4 + 1],colours[i * 4 + 2],colours[i * 4 + 3]));
            }
        }
        
        mesh.SetVertices(verticesList);
        mesh.triangles = triangles;
        mesh.uv = uvList.ToArray();
        mesh.colors = colourList.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }
}
