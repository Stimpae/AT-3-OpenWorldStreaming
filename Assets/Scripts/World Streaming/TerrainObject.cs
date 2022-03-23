using System;
using System.Collections.Generic;
using UnityEngine;

namespace World_Streaming
{
    public static class TerrainObject
    {
        public static GameObject CreateTerrainObject(Material mat)
        {
            var go = new GameObject {name = "terrain"};
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            var meshCollider = go.AddComponent<MeshCollider>();
        
            meshRenderer.material = mat;
            return go;
        }

        private static int GetLODAmount(ELevelOfDetail lod)
        {
            return lod switch
            {
                ELevelOfDetail.LOD0 => 4,
                ELevelOfDetail.LOD1 => 8,
                ELevelOfDetail.LOD2 => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(lod), lod, null)
            };
        }

        public static Mesh CreateTerrainMesh(ELevelOfDetail lod, int size,  Texture2D heightmap, Vector2 offset, float uvTiling, Gradient gradient)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var uvs = new List<Vector2>();
            var colors = new List<Color>();

            float minTerrainHeight = 0;
            float maxTerrainHeight = 200;
        
            // 0 // 8 // 16;
            var levelOfDetail = GetLODAmount(lod);

            var topLeftX = (size - 1) / -2f;
            var topLeftZ = (size - 1) / 2f;

            var meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
            var verticesPerLine = (size - 1) / meshSimplificationIncrement + 1;

            var vertexIndex = 0;

            for (var x = 0; x < size; x+= meshSimplificationIncrement)
            {
                for (var y = 0; y < size; y+= meshSimplificationIncrement)
                {
                    vertices.Add(new Vector3(topLeftX + x,heightmap.GetPixel(x + (int)offset.x,y + (int)offset.y).grayscale * 200,topLeftZ + y));
                    uvs.Add(new Vector2(x / (float)size * uvTiling,y / (float)size * uvTiling));

                    if (vertices[vertexIndex].y > maxTerrainHeight)
                        maxTerrainHeight = vertices[vertexIndex].y;
                    if (vertices[vertexIndex].y < minTerrainHeight)
                        minTerrainHeight = vertices[vertexIndex].y;
                
                    var height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight,vertices[vertexIndex].y);
                    colors.Add(gradient.Evaluate(height));
                
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
            
            var mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray(),
                colors = colors.ToArray()
            };
            
            mesh.RecalculateNormals();
            
            return mesh;
        }
    
    }
}
