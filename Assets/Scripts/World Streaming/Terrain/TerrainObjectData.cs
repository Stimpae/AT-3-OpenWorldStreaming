using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainObjectData
{
    // group all 3 lods together
    [SerializeField] public TerrainLODData lODData;
    [SerializeField] public TerrainLODData lODData01;
    [SerializeField] public TerrainLODData lODData02;

    // set main lod -> set lod data 01 -> set lod data 02
    public void SetLOD(string path, Mesh mesh)
    {
        MeshData meshData = new MeshData(mesh);
        lODData = new TerrainLODData(ELevelOfDetail.LOD0, path);
    }
}
