using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // create all of the LODS for the terrain for this partition 
        // log all of this lod data.. don't actually make the obects (just need the data
        
        // set the data for this terrain (overall object data, this contains the LOD data)
        // we can then move this terrain piece around, and just assign different data to it.
        m_meshRenderer = GetComponent<MeshRenderer>();
        m_meshFilter = GetComponent<MeshFilter>();
        
        m_triangles = new List<int>();
        m_vertices = new List<Vector3>();
        
        float vertexWidth = m_sizeXY / m_resolution;
        float vertexX = 0;
        float vertexY = 0;

        for (int x = 0; x < m_resolution; x++)
        {
            for (int y = 0; y < m_resolution; y++)
            {
                m_vertices.Add(new Vector3(vertexX,0,vertexY));
                vertexX += vertexWidth;
            }
            vertexY += vertexWidth;
            vertexX = 0;
        }
        
        for (int x = 0; x < m_resolution - 1; x++)
        {
            for (int y = 0; y < m_resolution -1; y++)
            {
                int vertexIndex = x * m_resolution + y;
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
    
    private void SetTerrainObjectData()
    {
        // feed in the object data. sets this terrain stuff to the default?
    }

    private void SetTerrainHeight()
    {
        // seperate function to just set the terrain height based
    }
    
}
