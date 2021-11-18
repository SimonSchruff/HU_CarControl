using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if(UNITY_EDITOR)
using UnityEditor;

[CustomEditor(typeof(QuestionRandomizer))]
public class QuestionRandEditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        QuestionRandomizer questionRandomizer = (QuestionRandomizer)target;

        if(GUILayout.Button("Randomize"))
        {
            questionRandomizer.RandomizeOnButtonPress(); 
        }
    }
}

#endif
