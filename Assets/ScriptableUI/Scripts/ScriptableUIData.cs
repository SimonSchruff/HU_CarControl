using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(menuName = "ScriptableUIData")]
public class ScriptableUIData : ScriptableObject
{
    public string text;

    [Header("Toggles")]
    public int toggleAmount;
    public string[] toggleDescriptions; 
}
