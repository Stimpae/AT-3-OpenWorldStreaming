using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace World_Creation
{
    public class WorldChunk : MonoBehaviour
    {
        public GameObject enemyPrefab;
        public GameObject treePrefab;
        public Color grassColour;
        public Color darkGrassColour;
        public Color edgeColour;
        public Material terrainMaterial;
        public Material waterMaterial;
        public int chunkSize = 100;
        public float scale = .1f;
        public float colourNoiseScale = .1f;
        public float treeNoiseScale = -.05f;
        public float treeDensity = .5f;
        public float waterLevel = .4f;

        private WorldTile[] m_tileGrid;
        private NavMeshSurface m_navMeshSurface;

        [ContextMenu("Start")]
        private void Start()
        {
            m_navMeshSurface = GetComponent<NavMeshSurface>();
            gameObject.tag = "Land";
            // check if this is loaded from a save if not then carry on generation from usual
            // need to get the seed somehow for all of the random shit happening

            float[] noiseMap = new float[chunkSize * chunkSize];
            float xNoiseOffset = Random.Range(-10000f, 10000f);
            float yNoiseOffset = Random.Range(-10000f, 10000f);
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float noise = Mathf.PerlinNoise(x * scale + xNoiseOffset, y * scale + yNoiseOffset);
                    noiseMap[y * chunkSize + x] = noise;
                }
            }

            float[] falloffMap = new float[chunkSize * chunkSize];
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float xv = x / (float) chunkSize * 2 - 1;
                    float yv = y / (float) chunkSize * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                    falloffMap[y * chunkSize + x] =
                        Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                }
            }

            m_tileGrid = new WorldTile[chunkSize * chunkSize];
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    WorldTile tile = new WorldTile();
                    float noise = noiseMap[y * chunkSize + x];
                    noise -= falloffMap[y * chunkSize + x];
                    tile.isWater = noise < waterLevel;

                    // 2d array here
                    m_tileGrid[y * chunkSize + x] = tile;
                }
            }

            DrawTerrainMesh(m_tileGrid);
            DrawWaterMesh();
            GenerateTrees(m_tileGrid);
            
            m_navMeshSurface.BuildNavMesh();
            
            SpawnEnemy();
        }

        private void DrawTerrainMesh(WorldTile[] grid)
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    WorldTile tile = grid[y * chunkSize + x];
                    if (!tile.isWater)
                    {
                        // create all of the vertex positions
                        // top face - grass
                        Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                        Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                        Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                        Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                        Vector3[] verticesA = new Vector3[] {a, b, c, b, d, c};
                        float noise = Mathf.PerlinNoise(x * colourNoiseScale, y * colourNoiseScale);
                      
                        for (int i = 0; i < 6; i++)
                        {
                            vertices.Add(verticesA[i]);
                            triangles.Add(triangles.Count);
                            Color newGrassColour = Color.Lerp(grassColour, darkGrassColour, noise * 2);
                            colors.Add(newGrassColour);
                        }

                        if (x > 0)
                        {
                            WorldTile leftTile = grid[y * chunkSize + (x - 1)];
                            if (leftTile.isWater)
                            {
                                var e = new Vector3(x - .5f, 0, y + .5f);
                                var f = new Vector3(x - .5f, 0, y - .5f);
                                var g = new Vector3(x - .5f, -10, y + .5f);
                                var h = new Vector3(x - .5f, -10, y - .5f);
                                Vector3[] verticesB = new Vector3[] {e, f, g, f, h, g};
                                for (int i = 0; i < 6; i++)
                                {
                                    vertices.Add(verticesB[i]);
                                    triangles.Add(triangles.Count);
                                    colors.Add(edgeColour);
                                }
                            }
                        }

                        if (x < chunkSize - 1)
                        {
                            WorldTile rightTile = grid[y * chunkSize + (x + 1)];
                            if (rightTile.isWater)
                            {
                                var e = new Vector3(x + .5f, 0, y - .5f);
                                var f = new Vector3(x + .5f, 0, y + .5f);
                                var g = new Vector3(x + .5f, -10, y - .5f);
                                var h = new Vector3(x + .5f, -10, y + .5f);
                                Vector3[] verticesB = new Vector3[] {e, f, g, f, h, g};
                                for (int i = 0; i < 6; i++)
                                {
                                    vertices.Add(verticesB[i]);
                                    triangles.Add(triangles.Count);
                                    colors.Add(edgeColour);
                                }
                            }
                        }

                        if (y > 0)
                        {
                            WorldTile frontTile = grid[(y - 1) * chunkSize + x];
                            if (frontTile.isWater)
                            {
                                var e = new Vector3(x - .5f, 0, y - .5f);
                                var f = new Vector3(x + .5f, 0, y - .5f);
                                var g = new Vector3(x - .5f, -10, y - .5f);
                                var h = new Vector3(x + .5f, -10, y - .5f);
                                Vector3[] verticesB = new Vector3[] {e, f, g, f, h, g};
                                for (int i = 0; i < 6; i++)
                                {
                                    vertices.Add(verticesB[i]);
                                    triangles.Add(triangles.Count);
                                    colors.Add(edgeColour);
                                }
                            }
                        }

                        if (y < chunkSize - 1)
                        {
                            WorldTile backTile = grid[(y + 1) * chunkSize + x];
                            if (backTile.isWater)
                            {
                                var e = new Vector3(x + .5f, 0, y + .5f);
                                var f = new Vector3(x - .5f, 0, y + .5f);
                                var g = new Vector3(x + .5f, -10, y + .5f);
                                var h = new Vector3(x - .5f, -10, y + .5f);
                                Vector3[] verticesB = new Vector3[] {e, f, g, f, h, g};
                                for (int i = 0; i < 6; i++)
                                {
                                    vertices.Add(verticesB[i]);
                                    triangles.Add(triangles.Count);
                                    colors.Add(edgeColour);
                                }
                            }
                        }
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = mesh;
            meshRenderer.material = terrainMaterial;
            meshFilter.sharedMesh = mesh;
        }

        private void SpawnEnemy()
        {
            int spawnedAmount = 0;
            while (true)
            {
                var position = gameObject.transform.position;
                Vector3 centre = new Vector3(position.x  + 50, 10, position.z + 50);
                Vector3 targetPos = RandomPointInBox(centre, new Vector3(100, 0, 100));
                if (Physics.Raycast(targetPos, -Vector3.up, out var hitPoint))
                {
                    if (hitPoint.transform.gameObject.CompareTag("Water"))
                    {
                        if (spawnedAmount >= 3)
                        {
                            break;
                        }
                        Instantiate(enemyPrefab, hitPoint.point, quaternion.identity);
                        spawnedAmount++;
                    }
                }
            }
        }
        
        private static Vector3 RandomPointInBox(Vector3 center, Vector3 size) {
 
            return center + new Vector3(
                (Random.value - 0.5f) * size.x,
                (Random.value - 0.5f) * size.y,
                (Random.value - 0.5f) * size.z
            );
        }
        
        private void DrawWaterMesh()
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var uvs = new List<Vector2>();
            var triangles = new List<int>();   
            
            var go = new GameObject();
            go.tag = "Water";
            MeshFilter meshFilter = go.AddComponent<MeshFilter>();
            MeshCollider meshCollider = go.AddComponent<MeshCollider>();
            MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
            meshRenderer.material = waterMaterial;
            
            go.transform.SetParent(this.transform);
            go.transform.localPosition = new Vector3(0,-.8f,0);
    
            var vertexIndex = 0;
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    vertices.Add(new Vector3(x,0,y));
                    uvs.Add(new Vector2(x / (float)chunkSize,y / (float)chunkSize));
                    
                    if (x < chunkSize - 1 && y < chunkSize - 1)
                    {
                        triangles.Add(vertexIndex + chunkSize);
                        triangles.Add(vertexIndex + chunkSize + 1);
                        triangles.Add(vertexIndex);
                        
                        triangles.Add(vertexIndex + 1);
                        triangles.Add(vertexIndex);
                        triangles.Add(vertexIndex + chunkSize + 1);
                    }

                    vertexIndex++;
                }
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        private void GenerateTrees(WorldTile[] grid)
        {
            float[] noiseMap = new float[chunkSize * chunkSize];
            float xNoiseOffset = Random.Range(-10000f, 10000f);
            float yNoiseOffset = Random.Range(-10000f, 10000f);
            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    float noise = Mathf.PerlinNoise(x * treeNoiseScale + xNoiseOffset, y * treeNoiseScale + yNoiseOffset);
                    noiseMap[y * chunkSize + x] = noise;
                }
            }

            for (int y = 0; y < chunkSize; y++)
            {
                for (int x = 0; x < chunkSize; x++)
                {
                    WorldTile tile = grid[y * chunkSize + x];
                    if (!tile.isWater)
                    {
                        float v = Random.Range(0f, treeDensity);
                        if (noiseMap[y * chunkSize + x] < v)
                        {
                            GameObject tree = Instantiate(treePrefab, transform);
                            tree.transform.localPosition = new Vector3(x,0,y);
                            tree.transform.rotation = Quaternion.Euler(0,Random.Range(0,360.0f),0);
                            tree.transform.localScale = Vector3.one * Random.Range(0.6f, 0.9f);
                        }
                    }

                }
            }
            
        }
        
    }
    
    
}
