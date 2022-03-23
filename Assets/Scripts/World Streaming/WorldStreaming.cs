using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStreaming : MonoBehaviour
{
    public GameObject player;

    private List<WorldChun> m_chunks = new List<WorldChun>();
    private WorldComposition m_worldComposition = null;
    
    // initiates the streaming with all of the necessary data
    public void InitStreaming(List<WorldChun> chunks, WorldComposition composition)
    {
        Debug.LogWarning("Streaming setup successfully");
        
        m_chunks = new List<WorldChun>();
        m_worldComposition = composition;
        m_chunks = chunks;
        GetObjectsInChunk();
    }
    
    // gets all of the objects inside of the trunk using a physics overlap
    // the size of these chunks
    public void GetObjectsInChunk()
    {
        // loops through each of the chunks 
        foreach (var chunk in m_chunks)
        {
            chunk.SetStaticObjectInChunk();
        }
    }
}
