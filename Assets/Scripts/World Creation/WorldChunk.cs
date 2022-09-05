using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace World_Creation
{
    // position of the chunk within world space
    // needs its own class
    public class ChunkCoordinate
    {
        public int x;
        public int z;

        public ChunkCoordinate(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public bool Equals(ChunkCoordinate other)
        {
            if (other == null)
            {
                return false;
            }
            else if (other.x == x && other.z == z)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    
    public class WorldChunk
    {
        private WorldTile[] m_tileGrid;
        private ChunkCoordinate m_chunkCoordinate;
        
        private readonly int m_chunkSize;
        private readonly GameObject m_chunkObject;
        private readonly World m_world;

        private readonly MeshRenderer m_meshRenderer;
        private readonly MeshFilter m_filter;
        private readonly MeshCollider m_collider;
        public Vector3 Position => m_chunkObject.transform.position;
        public ChunkData chunkData;
        
        // active setter and getter
        public bool IsActive
        {
            // need to change how this works i think..
            get { return m_chunkObject.activeSelf;}
            set { m_chunkObject.SetActive(value);}
            
        }
        
        // world chunk construction
        public WorldChunk(World world, ChunkCoordinate chunkCoords)
        {
            m_world = world;
            m_chunkCoordinate = chunkCoords;
            m_chunkSize = world.chunkSize;
            
            m_chunkObject = new GameObject();
            m_chunkObject.tag = "Land";
            m_chunkObject.transform.SetParent(world.transform);
            m_chunkObject.transform.position = new Vector3(chunkCoords.x * 99, 0f, chunkCoords.z * 99);
            m_chunkObject.name = "Chunk" + chunkCoords.x + " - " + chunkCoords.z;
            
            m_meshRenderer = m_chunkObject.AddComponent<MeshRenderer>();
            m_filter = m_chunkObject.AddComponent<MeshFilter>();
            m_collider = m_chunkObject.AddComponent<MeshCollider>();
            m_meshRenderer.material = m_world.terrainMaterial;
            
            chunkData = new ChunkData();
            GenerateChunkMeshData();
        }

       
        private void GenerateChunkMeshData()
        {
            // check if this is loaded from a save if not then carry on generation from usual
            // need to get the seed somehow for all of the random shit happening

            float[] noiseMap = new float[m_chunkSize * m_chunkSize];
            float xNoiseOffset = Random.Range(-10000f, 10000f);
            float yNoiseOffset = Random.Range(-10000f, 10000f);
            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    float noise = Mathf.PerlinNoise(x * m_world.scale + xNoiseOffset, y * m_world.scale + yNoiseOffset);
                    noiseMap[y * m_chunkSize + x] = noise;
                }
            }

            float[] falloffMap = new float[m_chunkSize * m_chunkSize];
            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    float xv = x / (float) m_chunkSize * 2 - 1;
                    float yv = y / (float) m_chunkSize * 2 - 1;
                    float v = Mathf.Max(Mathf.Abs(xv), Mathf.Abs(yv));
                    falloffMap[y * m_chunkSize + x] =
                        Mathf.Pow(v, 3f) / (Mathf.Pow(v, 3f) + Mathf.Pow(2.2f - 2.2f * v, 3f));
                }
            }

            m_tileGrid = new WorldTile[m_chunkSize * m_chunkSize];
            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    WorldTile tile = new WorldTile();
                    float noise = noiseMap[y * m_chunkSize + x];
                    noise -= falloffMap[y * m_chunkSize + x];
                    tile.isWater = noise < m_world.waterLevel;

                    // 2d array here
                    m_tileGrid[y * m_chunkSize + x] = tile;
                }
            }

            DrawTerrainMesh(m_tileGrid);
            DrawWaterMesh();
            GenerateTrees(m_tileGrid);
            SpawnEnemy();
        }

        private void DrawTerrainMesh(WorldTile[] grid)
        {
            var mesh = new Mesh();
            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();

            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    WorldTile tile = grid[y * m_chunkSize + x];
                    if (!tile.isWater)
                    {
                        // create all of the vertex positions
                        // top face - grass
                        Vector3 a = new Vector3(x - .5f, 0, y + .5f);
                        Vector3 b = new Vector3(x + .5f, 0, y + .5f);
                        Vector3 c = new Vector3(x - .5f, 0, y - .5f);
                        Vector3 d = new Vector3(x + .5f, 0, y - .5f);
                        Vector3[] verticesA = new Vector3[] {a, b, c, b, d, c};
                        float noise = Mathf.PerlinNoise(x * m_world.colourNoiseScale, y * m_world.colourNoiseScale);
                      
                        for (int i = 0; i < 6; i++)
                        {
                            vertices.Add(verticesA[i]);
                            triangles.Add(triangles.Count);
                            Color newGrassColour = Color.Lerp(m_world.grassColour, m_world.darkGrassColour, noise * 2);
                            colors.Add(newGrassColour);
                        }

                        if (x > 0)
                        {
                            WorldTile leftTile = grid[y * m_chunkSize + (x - 1)];
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
                                    colors.Add(m_world.edgeColour);
                                }
                            }
                        }

                        if (x < m_world.chunkSize - 1)
                        {
                            WorldTile rightTile = grid[y * m_chunkSize + (x + 1)];
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
                                    colors.Add(m_world.edgeColour);
                                }
                            }
                        }

                        if (y > 0)
                        {
                            WorldTile frontTile = grid[(y - 1) * m_chunkSize + x];
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
                                    colors.Add(m_world.edgeColour);
                                }
                            }
                        }

                        if (y < m_world.chunkSize - 1)
                        {
                            WorldTile backTile = grid[(y + 1) * m_chunkSize + x];
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
                                    colors.Add(m_world.edgeColour);
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
            
            m_collider.sharedMesh = mesh;
            m_meshRenderer.material = m_world.terrainMaterial;
            m_filter.sharedMesh = mesh;

            chunkData.chunkMeshData = new MeshData(mesh);
        }

        private void SpawnEnemy()
        {
            int spawnedAmount = 0;
            while (true)
            {
                var position = m_chunkObject.transform.position;
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
                        
                        Object.Instantiate(m_world.enemyPrefab, hitPoint.point, quaternion.identity);
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
            meshRenderer.material = m_world.waterMaterial;
            
            go.transform.SetParent(m_chunkObject.transform);
            go.transform.localPosition = new Vector3(0,-.8f,0);
            
            var vertexIndex = 0;
            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    vertices.Add(new Vector3(x,0,y));
                    uvs.Add(new Vector2(x / (float)m_chunkSize,y / (float)m_chunkSize));
                    
                    if (x < m_chunkSize - 1 && y < m_chunkSize - 1)
                    {
                        triangles.Add(vertexIndex + m_chunkSize);
                        triangles.Add(vertexIndex + m_chunkSize + 1);
                        triangles.Add(vertexIndex);
                        
                        triangles.Add(vertexIndex + 1);
                        triangles.Add(vertexIndex);
                        triangles.Add(vertexIndex + m_chunkSize + 1);
                    }
                    vertexIndex++;
                }
            }
            
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
            
            chunkData.chunkMeshData = new MeshData(mesh);
        }

        private void GenerateTrees(WorldTile[] grid)
        {
            float[] noiseMap = new float[m_chunkSize * m_chunkSize];
            float xNoiseOffset = Random.Range(-10000f, 10000f);
            float yNoiseOffset = Random.Range(-10000f, 10000f);
            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    float noise = Mathf.PerlinNoise(x * m_world.treeNoiseScale + xNoiseOffset, y * m_world.treeNoiseScale + yNoiseOffset);
                    noiseMap[y * m_chunkSize + x] = noise;
                }
            }

            for (int y = 0; y < m_chunkSize; y++)
            {
                for (int x = 0; x < m_chunkSize; x++)
                {
                    WorldTile tile = grid[y * m_chunkSize + x];
                    if (!tile.isWater)
                    {
                        float v = Random.Range(0f, m_world.treeDensity);
                        if (noiseMap[y * m_chunkSize + x] < v)
                        {
                            GameObject tree = GameObject.Instantiate(m_world.treePrefab, m_chunkObject.transform);
                            tree.transform.localPosition = new Vector3(x,0,y);
                            tree.transform.rotation = Quaternion.Euler(0,Random.Range(0,360.0f),0);
                            tree.transform.localScale = Vector3.one * Random.Range(0.6f, 0.9f);
                            
                            StaticObjectData objectData = new StaticObjectData(tree.transform.localPosition, tree.transform.eulerAngles, tree.transform.localScale);
                            objectData.prefabPath = AssetDatabase.GetAssetPath(m_world.treePrefab);
                            chunkData.staticObjectData.Add(objectData);
                        }
                    }

                }
            }
            
        }
        
    }
    
    
}
