using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if(UNITY_EDITOR)
using UnityEditor ;

[CustomEditor(typeof(ScriptableUIScale))]
public class ScriptabelUIScaleEditor : Editor
{
   
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //target is object currently inspected
        ScriptableUIScale scriptableUIScale = (ScriptableUIScale)target; 

        if (GUILayout.Button("Generate Toggles"))
        {
            scriptableUIScale.CreateToggles(); 

        }

        if (GUILayout.Button("Remove Toggles"))
        {
            scriptableUIScale.RemoveToggles(); 
        }
    }


}


#endif
