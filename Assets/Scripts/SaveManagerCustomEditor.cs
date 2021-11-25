using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if(UNITY_EDITOR)
using UnityEditor;

[CustomEditor(typeof(SQLSaveManager))]
public class SaveManagerCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SQLSaveManager sql = (SQLSaveManager)target;

        if(GUILayout.Button("Save Data"))
        {
            sql.StartPostCoroutine(); 
        }

    }
}

#endif
