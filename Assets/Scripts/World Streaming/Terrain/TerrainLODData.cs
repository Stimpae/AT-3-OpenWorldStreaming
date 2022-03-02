using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ELevelOfDetail
{
    LOD0,
    LOD1,
    LOD2
}

[System.Serializable]
public class TerrainLODData
{
    [SerializeField] private ELevelOfDetail m_lod;
    [SerializeField] private string m_path;
    //[SerializeField] private MeshData m_meshData;
    public ELevelOfDetail LOD => m_lod;
    public string Path => m_path;
    //public MeshData MeshData => m_meshData;

    public TerrainLODData(ELevelOfDetail lod, string path)
    {
        m_lod = lod;
        m_path = path;
    }
}
