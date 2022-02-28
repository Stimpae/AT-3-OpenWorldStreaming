using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPartitionData
{
    public int terrainHeightmapRes;
    public float[,] terrainHeights;

    public TerrainPartitionData(float[,] heights, int heightMapRes, int baseRes)
    {
        terrainHeights = heights;
        terrainHeightmapRes = heightMapRes;
    }
}



public class TerrainLODData
{
    
}

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class TerrainPartition : MonoBehaviour
{
    [Header("Settings")] 
    [SerializeField] private float m_sizeXY = 4;
    [SerializeField] private int m_resolution = 4;

    private MeshFilter m_meshFilter;
    private MeshRenderer m_meshRenderer;
    
    public List<Vector3> m_vertices = new List<Vector3>();
    public List<int> m_triangles = new List<int>();
    
    [ContextMenu("Create Terrain")]
    public void CreateTerrain()
    {
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshFilter = GetComponent<MeshFilter>();
        
        m_triangles = new List<int>();
        m_vertices = new List<Vector3>();
        
        float spacingAmount = m_sizeXY / m_resolution;
        float spacingX = 0;
        float spacingY = 0;

        for (int x = 0; x < m_resolution; x++)
        {
            for (int y = 0; y < m_resolution; y++)
            {
                m_vertices.Add(new Vector3(spacingX,0,spacingY));
                spacingX += spacingAmount;
            }
            
            spacingY += spacingAmount;
            spacingX = 0;
        }
        
        for (int x = 0; x < m_resolution; x++)
        {
            for (int y = 0; y < m_resolution; y++)
            {
                int vertexIndex = x * (m_resolution - 1) + y;
                m_triangles.Add(vertexIndex);
                m_triangles.Add(vertexIndex + m_resolution);
                m_triangles.Add(vertexIndex + m_resolution + 1);
                
                m_triangles.Add(vertexIndex);
                m_triangles.Add(vertexIndex + m_resolution + 1);
                m_triangles.Add(vertexIndex + 1);
            }
        }
        
        m_meshFilter.sharedMesh = new Mesh();
        m_meshFilter.sharedMesh.vertices = m_vertices.ToArray();
        m_meshFilter.sharedMesh.triangles = m_triangles.ToArray();
    }

    private void GenerateTerrainGrid()
    {
        
    }
    
    // Needs it own terrainPartitionData
    // Create the terrain

    // Set the terrains height
}
