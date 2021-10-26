using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class ScriptableUIScale : ScriptableUI
{
    int toggleNumber;
    string[] texts; 
    public Toggle toggle;
    GameObject instance; 

    protected override void OnSkinUI()
    {
        base.OnSkinUI();

        
        
    }

    public override void Awake()
    {
        base.Awake();

        toggleNumber = skinData.toggleAmount;
        texts = skinData.toggleDescriptions;
        Debug.Log(toggleNumber);

        //PROBLEM: Creates everytime you hit play
        Create("ToggleObj"); 

    }

    

    public void Create(string objName)
    {
        Debug.Log("Create"); 
        for(int i = 0; i < toggleNumber; i++)
        {
            instance = Instantiate(Resources.Load<GameObject>(objName));
            instance.name = objName + " " + i.ToString();
            
            

            
            instance.transform.SetParent(gameObject.transform, false);

            //Set pos of toggles
            //instance.transform.position = new Vector3(-100, 250 - (i * 50), 0 );
            // Set label according to Data Object
            

        }
    }
}
