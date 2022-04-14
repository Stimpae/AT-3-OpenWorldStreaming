using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Managers;
using Player;
using UnityEngine;
using UnityEngine.AI;
using World_Creation;
 
public class World : MonoBehaviour
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
    private NavMeshSurface m_navMeshSurface;
    
    // only 4 chunks rendered at the same time?
    private static readonly int m_worldSize = 20;
    private List<ChunkCoordinate> m_activeWorldChunks = new List<ChunkCoordinate>();
    private WorldChunk[] m_loadedChunks = new WorldChunk[m_worldSize * m_worldSize];
    private int m_viewDistanceInChunks = 2;

    private ChunkCoordinate m_currentPlayerChunkCoord;
    private ChunkCoordinate m_playerLastChunkCoord;

    private Dictionary<Vector2, string> m_chunkPaths;
    
    // Start is called before the first frame update
    void Start()
    {
        m_navMeshSurface = GetComponent<NavMeshSurface>();
        
        BuildWorld();

        Vector3 spawnPosition = new Vector3((m_worldSize * 99) / 2f, 5f, (m_worldSize * 99) / 2f);
        SpawnManager.Instance.SpawnPlayer(spawnPosition, 150.0f);

        m_playerLastChunkCoord = GetCoordinateFromVector3(PlayerController.Instance.transform.position);
    }

    private void BuildWorld()
    {
        for (int x = (m_worldSize / 2) - m_viewDistanceInChunks; x < (m_worldSize / 2) + m_viewDistanceInChunks; x++)
        {
            for (int z = (m_worldSize / 2) - m_viewDistanceInChunks; z < (m_worldSize / 2) + m_viewDistanceInChunks; z++)
            {
                CreateChunk(x,z);
            }
        }
        
        m_navMeshSurface.BuildNavMesh();
    }

    private void Update()
    {
        m_currentPlayerChunkCoord = GetCoordinateFromVector3(PlayerController.Instance.transform.position);
        if (!m_currentPlayerChunkCoord.Equals(m_playerLastChunkCoord))
        {
            CheckViewDistance();
        }
    }

    private void CreateChunk(int x, int z)
    {
        m_loadedChunks[x * m_worldSize + z] = new WorldChunk(this, new ChunkCoordinate(x,z));
        m_activeWorldChunks.Add(new ChunkCoordinate(x,z));
    }

    private void CheckViewDistance()
    {
        ChunkCoordinate coordinate = GetCoordinateFromVector3(PlayerController.Instance.transform.position);
        List<ChunkCoordinate> previouslyActiveChunk = new List<ChunkCoordinate>(m_activeWorldChunks);
        
        for (int x =  coordinate.x - m_viewDistanceInChunks; x < coordinate.x + m_viewDistanceInChunks; x++)
        {
            for (int z = coordinate.z - m_viewDistanceInChunks; z < coordinate.z + m_viewDistanceInChunks; z++)
            {
                if(IsChunkInWorld(new ChunkCoordinate(x,z)))
                {
                    if (m_loadedChunks[x * m_worldSize + z] == null)
                    {
                        CreateChunk(x,z);
                        m_navMeshSurface.BuildNavMesh();
                    }
                    else if (!m_loadedChunks[x * m_worldSize + z].IsActive)
                    {
                        m_loadedChunks[x * m_worldSize + z].IsActive = true;
                        m_activeWorldChunks.Add(new ChunkCoordinate(x,z));
                    }
                }

                for (int i = 0; i < previouslyActiveChunk.Count; i++)
                {
                    if (previouslyActiveChunk[i].Equals(new ChunkCoordinate(x, z)))
                        previouslyActiveChunk.RemoveAt(i);
                }
            }
        }

        foreach (var chunk in previouslyActiveChunk)
            m_loadedChunks[chunk.x * m_worldSize + chunk.z].IsActive = false;
    }

    private bool IsChunkInWorld(ChunkCoordinate coordinate)
    {
        if (coordinate.x > 0 && coordinate.x < m_worldSize - 1 && coordinate.z > 0 && coordinate.z < m_worldSize - 1)
            return true;
        else return false;
    }

    private ChunkCoordinate GetCoordinateFromVector3(Vector3 pos)
    {
        int x = Mathf.FloorToInt(pos.x / 99);
        int z = Mathf.FloorToInt(pos.z / 99);
        return new ChunkCoordinate(x,z);
    }

    private void UnloadChunk()
    {
        
    }

    private void LoadChunk()
    {
        
    }

    
}
