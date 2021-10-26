using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ToggleManager : MonoBehaviour
{
    public Toggle[] toggles;
    public string[] toggleDescriptions; 

    private void Awake()
    {
        string[] texts;

        toggles = GetComponentsInChildren<Toggle>();

        for(int i = 0; i <= toggles.Length; i++)
        {
            //toggles.
        }
        
        
    }


}
