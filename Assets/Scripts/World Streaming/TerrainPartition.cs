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
    
    public void CreateTerrain(int size,Texture2D heightmap, Vector2 offset)
    {
        //GameObject go = new GameObject();
        _terrainObjectData = new TerrainObjectData();

        _meshFilter = GetComponent<MeshFilter>();
        _meshRenderer = GetComponent<MeshRenderer>();

        //newHeight = height;
        
        CreateTerrainData(ELevelOfDetail.LOD0, size, heightmap, offset);
        //CreateTerrainData(ELevelOfDetail.LOD1, size, terrainResolution / 2, height);
        //CreateTerrainData(ELevelOfDetail.LOD2, size, terrainResolution / 4, height);
        
        //terrainObjectData = _terrainObjectData;
        //_meshFilter.sharedMesh = terrainObjectData.lODData.MeshData.GetMesh();
    }
    
    private void CreateTerrainData(ELevelOfDetail lod, int size,  Texture2D heightmap, Vector2 offset)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        // 0 // 8 // 16;
        int levelOfDetail = 0;

        float topLeftX = (size - 1) / -2f;
        float topLeftZ = (size - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (size - 1) / meshSimplificationIncrement + 1;

        int vertexIndex = 0;

        for (int x = 0; x < size; x+= meshSimplificationIncrement)
        {
            for (int y = 0; y < size; y+= meshSimplificationIncrement)
            {
                vertices.Add(new Vector3(topLeftX + x,heightmap.GetPixel(x + (int)offset.x,y + (int)offset.y).grayscale * 200,topLeftZ + y));
                
                if (x < size - 1 && y < size - 1)
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + verticesPerLine + 1);
                    triangles.Add(vertexIndex + verticesPerLine);
                
                    triangles.Add(vertexIndex + verticesPerLine + 1);
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        _meshFilter.sharedMesh = mesh;
        // returns mesh

        //TerrainLODData data = new TerrainLODData(ELevelOfDetail.LOD0, "hfdsfd");
        
        // figure out a more appropriate place to save the mesh data?
        // calculate the lods at runtime?
        
        //MeshData meshData = new MeshData(mesh);
        //StartCoroutine(StartSomething(mesh));
    }

    IEnumerator StartSomething(Mesh mesh)
    {
        yield return new WaitForSeconds(5);
        //MeshData meshData = new MeshData(mesh);
    }
}
