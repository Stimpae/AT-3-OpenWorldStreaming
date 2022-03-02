using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.TerrainAPI;

public class WorldStreaming : MonoBehaviour
{
    public Texture2D terrainTexture;
    public GameObject partitionObject;
    public int terrainSectionWidth = 100;
    public int terrainSectionHeight = 100;
    public int tilingAmount = 14;
    public Camera player;

    private List<WorldPartition> m_sections = new List<WorldPartition>();
    private List<Terrain> m_terrainSections = new List<Terrain>();
    private Vector2 m_sectionSize;

    private void Start()
    {
        BuildWorld();
    }
    
    private void Update()
    {
        /*
        foreach (var section in m_sections)
        {
            if (Vector3.Distance(player.transform.position, section.GetPartitionPosition()) >  100 * 2)
            {
                if (section.loaded)
                {
                    section.UnloadTerrain();
                    //section.UnloadTerrain();
                }
            }
            else
            {
                if(!section.loaded)
                {
                    section.LoadTerrain();
                    //section.LoadTerrain();
                }
            }
        }
        */
    }


    
    private void CheckDistance()
    {
        
    }
    
    private void LoadPartition()
    {
        
    }

    private void UnloadPartitionOutOfRange()
    {
        
    }

    [ContextMenu("Build World Partitions")]
    private void BuildWorld()
    {
        // get the source dimensions
        var sourceWidth = terrainTexture.width;
        var sourceHeight = terrainTexture.height;
        int terrainResolution = sourceWidth / tilingAmount;
        
        var tileSize = new Vector2Int(terrainResolution + 1, terrainResolution + 1);

        // Check how often the given tileSize fits into the image
        var tileAmountX = Mathf.FloorToInt(sourceWidth / (float) tileSize.x);
        var tileAmountY = Mathf.FloorToInt(sourceHeight / (float) tileSize.y);
        
        for (var y = 0; y < tileAmountY; y++)
        {
            for (var x = 0; x < tileAmountX; x++)
            {
                Vector3 position = new Vector3(x * terrainSectionWidth, 0.0f, y * terrainSectionHeight);
                
                /*
                // get the bottom left pixel coordinate of the current tile
                var bottomLeftPixelX = x * tileSize.x;
                var bottomLeftPixelY = y * tileSize.y;

                // get the pixels in the rect for this tile
                var pixels = terrainTexture.GetPixels(bottomLeftPixelX, bottomLeftPixelY, tileSize.x, tileSize.y);

                // create the new texture (adjust the additional parameters according to your needs)
                var texture = new Texture2D(tileSize.x, tileSize.y, terrainTexture.format, false);

                // write the pixels into the new texture
                texture.SetPixels(pixels);
                texture.Apply();
                */

                var go = Instantiate(partitionObject, this.gameObject.transform);
                go.name = "Partition - " + x + "," + y;
                go.GetComponent<WorldPartition>().InstantiateWorldPartition(terrainTexture, position.z, position.x);
                m_sections.Add(go.GetComponent<WorldPartition>());
            }
        }
    }
}

public class PartitionCoordinate
{
    public int x;
    public int z;

    public PartitionCoordinate(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}
    
    

