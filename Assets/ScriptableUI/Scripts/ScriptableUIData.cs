using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "ScriptableUIData")]
public class ScriptableUIData : ScriptableObject
{
    public string text;

    
    [System.Serializable]
    public struct ToggleData
    {
        public Vector2 startPos;
        public int spacing; 
        public Color textColor; 
        public bool hasFreeTextField;
        public bool vertical;
        public string objName; 
        public int toggleAmount;
        public string[] toggleDescriptions;
        

    }
    public ToggleData toggleData; 
}
