using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using World_Creation;

[CustomEditor(typeof(WorldChunk))]
public class WorldCompositionEditor : Editor
{
    private WorldComposition m_worldComposition;
    private Vector3 previewPosition;
    private int paletteSelection = 0;
    private Rect paletteRect;
    public GameObject[] palette = new GameObject[0];
    
}
