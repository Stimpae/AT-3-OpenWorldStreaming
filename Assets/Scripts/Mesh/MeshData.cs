using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MeshData
{
    // store everything in 1 dimensional arrays.
    [SerializeField] public List<float> vertices;
    [SerializeField] public List<int> triangles;
    [SerializeField] public List<float> uv;
    [SerializeField] public List<float> normals;

    // take in the mesh and start constructing data fields
    public MeshData(Mesh mesh)
    {
        vertices = new List<float>();
        triangles = new List<int>();
        uv = new List<float>();
        normals = new List<float>();
        
        // loop through all of the mesh data
    }
}
