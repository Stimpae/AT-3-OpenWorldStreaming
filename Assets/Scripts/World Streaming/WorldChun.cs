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
using World_Streaming;

public class WorldChun : MonoBehaviour
{
    [SerializeField] private Material worldMaterial;
    [SerializeField] private ELevelOfDetail levelOfDetail;
    
    private Texture2D m_heightmap;
    private Vector2 m_position = new Vector2();
    private GameObject m_worldPartitionTerrain;
    
    private bool m_instantiated;
    private string m_worldPath;

    private TerrainObjectData m_terrainData;
    private List<MeshData> m_staticObjectMeshData = new List<MeshData>();
    private List<GameObject> m_staticGameObjects = new List<GameObject>();
    
    public LayerMask m_LayerMask;
    
    public bool IsActive { get; set; }

    public void Awake()
    {
        m_worldPath = Application.persistentDataPath;
    }

    public void InstantiateWorldChunk(Texture2D heightmap, int size, float xPos, float yPos, float uvTiling, Gradient gradient)
    {
        m_instantiated = true;
        m_heightmap = heightmap;
        m_position = new Vector2(xPos, yPos);

        transform.position = new Vector3(m_position.x, 0.0f, m_position.y);

        m_worldPartitionTerrain = TerrainObject.CreateTerrainObject(worldMaterial);
        m_worldPartitionTerrain.transform.SetParent(transform);
        m_worldPartitionTerrain.transform.localPosition = Vector3.zero;
        
        Mesh meshLod = TerrainObject.CreateTerrainMesh(ELevelOfDetail.LOD0, size + 1, heightmap, m_position, uvTiling, gradient);
        Mesh meshLod1 = TerrainObject.CreateTerrainMesh(ELevelOfDetail.LOD1, size + 1, heightmap, m_position, uvTiling, gradient);
        
        m_terrainData = new TerrainObjectData();
        m_terrainData.SetLOD(meshLod,meshLod1);
        UpdateLOD(ELevelOfDetail.LOD0);
    }
    
    public void UpdateLOD(ELevelOfDetail lod)
    {
        switch (lod)
            {
                case ELevelOfDetail.LOD0:
                    m_worldPartitionTerrain.GetComponent<MeshFilter>().sharedMesh = m_terrainData.GetDataOne().GetMesh();
                    m_worldPartitionTerrain.GetComponent<MeshCollider>().sharedMesh = m_terrainData.GetDataOne().GetMesh();
                    break;
                case ELevelOfDetail.LOD1:
                    m_worldPartitionTerrain.GetComponent<MeshFilter>().sharedMesh =
                        m_terrainData.GetDataTwo().GetMesh();
                    break;
                case ELevelOfDetail.LOD2:
                    
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
    
    // set all of the stationary objects in the chunk
    public void SetStaticObjectInChunk()
    {
        // hard coded values for the moment
        Vector3 center = new Vector3(GetPartitionPosition().x, 64, GetPartitionPosition().z + 64);
        Vector3 radius = new Vector3(31.5f,128,31.5f);
        
        Collider[] hitColliders = Physics.OverlapBox(center, radius,Quaternion.identity, m_LayerMask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            m_staticObjectMeshData.Add(new MeshData(hitColliders[i].GetComponent<MeshFilter>().mesh));
            m_staticGameObjects.Add(hitColliders[i].gameObject);
        }
        
        m_terrainData.StaticObjectMeshData = m_staticObjectMeshData;
        DeactivateChunk();
    }
    
    // activate chunk -> load in mesh data
    public void ActiveChunk()
    {
        m_terrainData = (TerrainObjectData)DataManager.LoadData(m_worldPath);
        if (m_terrainData != null)
        {
            UpdateLOD(ELevelOfDetail.LOD0);
            for (int i = 0; i < m_staticGameObjects.Count; i++)
            {
                if (m_staticGameObjects[i] != null)
                {
                    m_staticGameObjects[i].GetComponent<MeshFilter>().mesh = m_terrainData.StaticObjectMeshData[i].GetMesh();
                }
            }
        }
    }

    // deactivate chunk -> save mesh data (only needs to happen once) (save static chunk)
    public void DeactivateChunk()
    {
        DataManager.SaveData(out m_worldPath, m_terrainData, gameObject.name);
        UpdateLOD(ELevelOfDetail.LOD2);
        
        foreach (var staticGameObject in m_staticGameObjects)
        {
            if (staticGameObject)
            {
                staticGameObject.GetComponent<MeshFilter>().mesh = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsActive = true;
            ActiveChunk();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IsActive = false;
            DeactivateChunk();
        }
    }
}

public class WorldCoordinate
{
    public int x;
    public int z;

    public WorldCoordinate(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}
