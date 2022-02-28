using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

[System.Serializable]
public class PartitionData
{
    public int terrainHeightmapRes;
    public float[,] terrainHeights;

    public PartitionData(float[,] heights, int heightMapRes, int baseRes)
    {
        terrainHeights = heights;
        terrainHeightmapRes = heightMapRes;
    }
}

public class WorldPartition : MonoBehaviour
{
    private Texture2D m_heightmap;
    private Vector2 m_position = new Vector2();
    private GameObject m_worldPartitionTerrain;

    private string m_path;
    private TerrainData m_terrainData;

    private PartitionData m_data;
    public bool loaded;

    public Terrain GetPartitionTerrain()
    {
        return m_worldPartitionTerrain.GetComponent<Terrain>();
        
    }

    [ContextMenu("Unload Terrain")]
    public void UnloadTerrain()
    {
        loaded = false;
        DataManager.SaveData(out m_path, m_data, gameObject.name);
        Destroy(m_worldPartitionTerrain);
    }

    [ContextMenu("Load Terrain")]
    public void LoadTerrain()
    {
        loaded = true;
        var data = (PartitionData)DataManager.LoadData(m_path);
        
        TerrainData tData = new TerrainData();
        tData.heightmapResolution = data.terrainHeightmapRes;
        tData.baseMapResolution = 128;
        tData.SetHeights(0, 0, data.terrainHeights);
        tData.size = new Vector3(50, 100, 50);

        m_worldPartitionTerrain = Terrain.CreateTerrainGameObject(tData);
        m_worldPartitionTerrain.transform.SetParent(this.transform);
        m_worldPartitionTerrain.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void InstantiateWorldPartition(WorldStreaming world, Texture2D heightmap, float xPos, float yPos)
    {
        m_heightmap = heightmap;
        m_position = new Vector2(xPos, yPos);

        transform.position = new Vector3(m_position.x, 0.0f, m_position.y);

        TerrainData tData = new TerrainData();
        tData.heightmapResolution = 128;
        tData.baseMapResolution = 128;

        float[,] heights = new float[heightmap.width, heightmap.height];

        for (int y = 0; y < heightmap.height; y++)
        {
            for (int x = 0; x < heightmap.width; x++)
            {
                heights[x, y] = m_heightmap.GetPixel(x, y).grayscale;
            }
        }

        tData.SetHeights(0, 0, heights);
        tData.size = new Vector3(50, 100, 50);

        m_worldPartitionTerrain = Terrain.CreateTerrainGameObject(tData);

        m_data = new PartitionData(tData.GetHeights(0, 0, 129, 129), 128, 128);

        m_worldPartitionTerrain.transform.SetParent(this.transform);
        m_worldPartitionTerrain.transform.localPosition = new Vector3(0, 0, 0);
    }

    public Vector3 GetPartitionPosition()
    {
        return new Vector3(m_position.x, 0, m_position.y);
    }
    
}