using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ChunkData
{
    public MeshData chunkMeshData;
    public List<StaticObjectData> staticObjectData = new List<StaticObjectData>();
}
