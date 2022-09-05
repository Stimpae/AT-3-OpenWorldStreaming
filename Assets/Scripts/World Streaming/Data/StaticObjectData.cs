using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class StaticObjectData
{
    public string prefabPath;
    public float[] position;
    public float[] rotation;
    public float[] scale;

    public StaticObjectData(Vector3 _position, Vector3 _rotation, Vector3 _scale)
    {
        position = new float[3];
        position[0] = _position.x;
        position[1] = _position.y;
        position[2] = _position.z;
        
        rotation = new float[3];   
        rotation[0] = _rotation.x;
        rotation[1] = _rotation.y;
        rotation[2] = _rotation.z;
        
        scale = new float[3];
        scale[0] = _scale.x;
        scale[1] = _scale.y;
        scale[2] = _scale.z;
    }
    
}
