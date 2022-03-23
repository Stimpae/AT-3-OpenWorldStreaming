using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class WorldComposition: MonoBehaviour
{
    [Header("World Terrain Settings"), Space(5f)]
    public Texture2D terrainHeightmap;
    public GameObject chunkObject;
    public int chunkWidth = 100;
    public int chunkHeight = 100;
    public int tilingAmount = 14;
    public int uvTiling = 10;

    [Header("World Terrain Style")] 
    public Gradient gradient;
    public GameObject placementObject;

    private List<WorldChun> m_chunks;
    private WorldStreaming m_streaming;
    
    private void Start()
    {
        BuildWorld();
        // logs all of the objects that are in the trunk on begin.
        m_streaming = GameObject.Find("World Streaming").GetComponent<WorldStreaming>();
        if (m_streaming)
        {
            // inits the streaming with the chunk data and composition reference
            m_streaming.InitStreaming(m_chunks, this);
        }
    }
    
    public void BuildWorld()
    {
        // destroys the previous world
        RemovePreviousWorld();
        m_chunks = new List<WorldChun>();
        
        // get the source dimensions
        var sourceWidth = terrainHeightmap.width;
        var sourceHeight = terrainHeightmap.height;
        int terrainResolution = sourceWidth / tilingAmount;
        
        var tileSize = new Vector2Int(terrainResolution + 1, terrainResolution + 1);

        // Check how often the given tileSize fits into the image
        var tileAmountX = Mathf.FloorToInt(sourceWidth / (float) tileSize.x);
        var tileAmountY = Mathf.FloorToInt(sourceHeight / (float) tileSize.y);
        
        for (var y = 0; y < tileAmountY; y++)
        {
            for (var x = 0; x < tileAmountX; x++)
            {
                Vector3 position = new Vector3(x * chunkWidth, 0.0f, y * chunkHeight);
                
                var go = Instantiate(chunkObject, this.gameObject.transform);
                go.name = "Partition-" + x + y;
                go.GetComponent<WorldChun>().InstantiateWorldChunk(terrainHeightmap, chunkWidth, position.z, position.x, uvTiling, gradient);
                m_chunks.Add(go.GetComponent<WorldChun>());
            }
        }
    }

    // removes all of the previously created segments, used when you are rebuilding
    // the terrain after changes
    private void RemovePreviousWorld()
    {
        int children = transform.childCount;
        if (children > 0)
        {
            for (int i = children - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }
                else
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}


    

