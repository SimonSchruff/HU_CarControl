using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Prevents Build Errors
#if (UNITY_EDITOR)

using UnityEditor;



public class ScriptableUIInstance : Editor
{
    [MenuItem("GameObject/ScriptableUI/Text", priority = 0 )]
    public static void AddText()
    {
        Create("TextObject"); 
    }
    static GameObject textObject; 

    [MenuItem("GameObject/ScriptableUI/ToggleGroup", priority = 0)]
    public static void AddToggleGroup()
    {
        Create("ToggleGroup"); 
    }
    static GameObject toggleGroupObj; 

    private static GameObject Create(string objName)
    {
        GameObject instance = Instantiate(Resources.Load<GameObject> (objName)); 

        instance.name = objName; 
        textObject = UnityEditor.Selection.activeObject as GameObject; 

        if(textObject != null)
        {
            instance.transform.SetParent(textObject.transform, false); 

        }

        return instance; 
    }
}

#endif
