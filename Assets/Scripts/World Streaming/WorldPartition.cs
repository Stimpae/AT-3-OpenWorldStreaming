using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class WorldPartition : MonoBehaviour
{
    [SerializeField] private Material worldMaterial;
    [SerializeField] private ELevelOfDetail levelOfDetail;
    
    private Texture2D m_heightmap;
    private Vector2 m_position = new Vector2();
    private GameObject m_worldPartitionTerrain;

    private Mesh meshLOD0;
    private Mesh meshLOD01;
    private Mesh meshLOD02;
    
    public void Awake()
    {
        meshLOD0 = new Mesh();
        meshLOD01 = new Mesh();
        meshLOD02 = new Mesh();
    }

    public void InstantiateWorldPartition(Texture2D heightmap, float xPos, float yPos)
    {
        m_heightmap = heightmap;
        m_position = new Vector2(xPos, yPos);

        transform.position = new Vector3(m_position.x, 0.0f, m_position.y);

        m_worldPartitionTerrain = TerrainPartition.CreateTerrainObject(129, heightmap, worldMaterial, m_position);
        m_worldPartitionTerrain.transform.SetParent(transform);
        m_worldPartitionTerrain.transform.localPosition = Vector3.zero;
        
        meshLOD0 = TerrainPartition.CreateTerrainMesh(ELevelOfDetail.LOD0, 129, heightmap, m_position);
        meshLOD01 = TerrainPartition.CreateTerrainMesh(ELevelOfDetail.LOD1, 129, heightmap, m_position);
        meshLOD02 = TerrainPartition.CreateTerrainMesh(ELevelOfDetail.LOD2, 129, heightmap, m_position);
        
        MeshData meshData = new MeshData(meshLOD0);

        UpdateLOD(ELevelOfDetail.LOD0);
    }
    
    [ContextMenu("Update")]
    private void Something()
    {
        UpdateLOD(levelOfDetail);
    }

    public void UpdateLOD(ELevelOfDetail lod)
    {
        switch (lod)
        {
            case ELevelOfDetail.LOD0:
                m_worldPartitionTerrain.GetComponent<MeshFilter>().sharedMesh = meshLOD0;
                break;
            case ELevelOfDetail.LOD1:
                m_worldPartitionTerrain.GetComponent<MeshFilter>().sharedMesh = meshLOD01;
                break;
            case ELevelOfDetail.LOD2:
                m_worldPartitionTerrain.GetComponent<MeshFilter>().sharedMesh = meshLOD02;
                break;
            case ELevelOfDetail.LOD3:
                m_worldPartitionTerrain.GetComponent<MeshFilter>().sharedMesh = null;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(lod), lod, null);
        }
    }

    public Vector3 GetPartitionPosition()
    {
        return new Vector3(m_position.x, 0, m_position.y);
    }
}