using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainPartition : MonoBehaviour
{
    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    private TerrainObjectData _terrainObjectData;
    
    public List<float> newHeight = new List<float>();
    
    public void CreateTerrain(float size, int terrainResolution, Texture2D heightmap, Vector2 offset)
    {
        //GameObject go = new GameObject();
        _terrainObjectData = new TerrainObjectData();

        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        //newHeight = height;
        
        CreateTerrainData(ELevelOfDetail.LOD0, size, terrainResolution, heightmap, offset);
        //CreateTerrainData(ELevelOfDetail.LOD1, size, terrainResolution / 2, height);
        //CreateTerrainData(ELevelOfDetail.LOD2, size, terrainResolution / 4, height);
        
        //terrainObjectData = _terrainObjectData;
        //_meshFilter.sharedMesh = terrainObjectData.lODData.MeshData.GetMesh();
    }
    
    private void CreateTerrainData(ELevelOfDetail lod, float size, int terrainResolution,  Texture2D heightmap, Vector2 offset)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        float vertexWidth = size / terrainResolution;
        float vertexX = 0;
        float vertexY = 0;
        
        // this needs to be rewritten, similar https://www.youtube.com/watch?v=417kJGPKwDg&ab_channel=SebastianLague
        // take this code and adapt it to what i need to calculate the lod (not this yet but to skip the verticies/height
        // to get the correct resolution
        
        // need to also correct the positioning of the units to be exactly 100 x 100
        
        // and also need to recalculate the normals and to calculate the uvs
        for (int x = 0; x < terrainResolution; x++)
        {
            for (int y = 0; y < terrainResolution; y++)
            {
                vertices.Add(new Vector3(vertexX,heightmap.GetPixel(x + (int)offset.y,y + (int)offset.x).grayscale * 100,vertexY));
                vertexX += vertexWidth;

                if (x < terrainResolution - 1 && y < terrainResolution - 1)
                {
                    int vertexIndex = x * terrainResolution + y;
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + terrainResolution);
                    triangles.Add(vertexIndex + terrainResolution + 1);
                
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + terrainResolution + 1);
                    triangles.Add(vertexIndex + 1);
                }
            }
            vertexY += vertexWidth;
            vertexX = 0;
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        _meshFilter.sharedMesh = mesh;

        //TerrainLODData data = new TerrainLODData(ELevelOfDetail.LOD0, "hfdsfd");
        
        // figure out a more appropriate place to save the mesh data?
        // calculate the lods at runtime?
        
        //MeshData meshData = new MeshData(mesh);
    }
}
