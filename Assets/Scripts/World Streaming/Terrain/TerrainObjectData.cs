using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainObjectData
{
    // can tidy all of this up
    // group all 3 lods together
    public MeshData meshLodData;
    public MeshData meshLodDataTwo;
    public List<MeshData> StaticObjectMeshData { get; set; }
    
    public MeshData GetDataOne()
    {
        return meshLodData;
    }

    public MeshData GetDataTwo()
    {
        return meshLodDataTwo;
    }
    
    // set main lod -> set lod data 01 -> set lod data 02
    public void SetLOD(Mesh mesh, Mesh meshTwo)
    {
        meshLodData = new MeshData(mesh);
        meshLodDataTwo = new MeshData(meshTwo);
    }
}
