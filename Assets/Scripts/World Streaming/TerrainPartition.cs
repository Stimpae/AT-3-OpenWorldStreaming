using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainPartition
{
    public static GameObject CreateTerrainObject(int size,Texture2D heightmap, Material mat, Vector2 offset)
    {
        GameObject go = new GameObject();
        

        MeshFilter meshFilter;
        MeshRenderer meshRenderer;
        meshFilter = go.AddComponent<MeshFilter>();
        meshRenderer = go.AddComponent<MeshRenderer>();

        meshRenderer.material = mat;
        return go;
    }

    private static int GetLODAmount(ELevelOfDetail lod)
    {
        switch (lod)
        {
            case ELevelOfDetail.LOD0:
                return 2;
            case ELevelOfDetail.LOD1:
                return 4;
            case ELevelOfDetail.LOD2:
                return 8;
            default:
                throw new ArgumentOutOfRangeException(nameof(lod), lod, null);
        }
    }
    
    public static Mesh CreateTerrainMesh(ELevelOfDetail lod, int size,  Texture2D heightmap, Vector2 offset)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        
        // 0 // 8 // 16;
        int levelOfDetail = GetLODAmount(lod);

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

        return mesh;
    }
    
}
