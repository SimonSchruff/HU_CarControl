using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


[RequireComponent(typeof(Text))]

public class ScriptableUIText : ScriptableUI
{
    
    Text text; 
    

    protected override void OnSkinUI()
    {
        base.OnSkinUI(); 

        text = GetComponent<Text>(); 

        if(text != null)
        {
            text.text = skinData.text; 
        }

    }

}
