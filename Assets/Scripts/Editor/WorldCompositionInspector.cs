using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WorldComposition))]
public class WorldCompositionInspector : Editor
{
    private WorldComposition m_worldComposition;
    private Vector3 previewPosition;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        m_worldComposition = (WorldComposition) target;
        
        EditorGUILayout.Space(10);

        if (GUILayout.Button("Build World"))
        {
            if (m_worldComposition)
            {
                m_worldComposition.BuildWorld();
            }
        }
        
        EditorGUILayout.Space(10);
    }
    
    private void OnSceneGUI()
    {
        m_worldComposition = (WorldComposition) target;
        Event e = Event.current;
        Place(e);
        Selection.activeGameObject = m_worldComposition.gameObject;
        SceneView.RepaintAll();
    }
    
    public void Place(Event e)
    {
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Debug.Log("Click");
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit)) return;
            previewPosition = hit.point;
            Vector3 defaultEulers = m_worldComposition.placementObject.transform.eulerAngles;
            GameObject instance = PrefabUtility.InstantiatePrefab(m_worldComposition.placementObject) as GameObject;
            instance.transform.position = hit.point;
            instance.transform.rotation = Quaternion.Euler(defaultEulers.x, SceneView.lastActiveSceneView.camera.transform.rotation.eulerAngles.y + defaultEulers.y, defaultEulers.z);
            instance.name = m_worldComposition.placementObject.name;
            
        }
    }

   
}
