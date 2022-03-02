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
    [SerializeField] private GameObject m_terrainPartition;
    
    private Texture2D m_heightmap;
    private Vector2 m_position = new Vector2();
    private GameObject m_worldPartitionTerrain;
    
    private TerrainObjectData m_terrainData;
    private List<float> heights = new List<float>();
    
    public void InstantiateWorldPartition(Texture2D heightmap, float xPos, float yPos)
    {
        m_terrainData = new TerrainObjectData();
        m_heightmap = heightmap;
        m_position = new Vector2(xPos, yPos);

        transform.position = new Vector3(m_position.x, 0.0f, m_position.y);

        m_worldPartitionTerrain = Instantiate(m_terrainPartition, this.transform);
        m_worldPartitionTerrain.GetComponent<TerrainPartition>().CreateTerrain(200 + 1, heightmap, m_position);
    }
}