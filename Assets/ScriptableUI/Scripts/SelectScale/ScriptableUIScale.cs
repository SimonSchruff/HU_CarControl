using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class ScriptableUIScale : ScriptableUI
{
    //Variables etc
    int toggleNumber;
    string[] texts;
    Vector2 startPos;
    string objName;
    Color color; 

    //Obj Ref
    Canvas canvas;
    GameObject inputField; 

    //Bools
    bool hasFreeTextField = false;
    bool vertical; 

    public void SetInputFieldInteractable(bool interactable)
    {
        
        if(inputField != null)
        {
            InputField iF = inputField.GetComponentInChildren<InputField>();

            if (interactable)
                iF.interactable = true;
            else
                iF.interactable = false;


        }
        else
        {
            Debug.Log("No Input Field Set!"); 
        }



    }




    #region Inspector Button Events
    public void CreateToggles()
    {
        //Get All variables from ScriptableUIData
        toggleNumber = skinData.toggleData.toggleAmount;
        texts = skinData.toggleData.toggleDescriptions;
        hasFreeTextField = skinData.toggleData.hasFreeTextField;
        startPos = skinData.toggleData.startPos;
        objName = skinData.toggleData.objName;
        color = skinData.toggleData.textColor;
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>(); 

        
        for (int i = 0; i < toggleNumber; i++)
        {
            
            GameObject instance = Instantiate(Resources.Load<GameObject>(objName));
            instance.name = objName + " " + i.ToString();
            instance.GetComponentInChildren<Text>().text = texts[i];
            instance.GetComponentInChildren<Text>().color = color;
            instance.GetComponent<Toggle>().isOn = false; 
            instance.transform.SetParent(gameObject.transform, false);

            //Spacing als variabel
            //X statt Y Pos verändern wenn bool vertical false



            //problem: takes canvas pos and adds up start pos instead of taking start pos as position
            Vector3 canvasPos = new Vector3(canvas.transform.position.x, canvas.transform.position.y , 0 );
            print(canvasPos);

            instance.transform.localPosition = new Vector3(startPos.x, startPos.y - (i * 50), 0); 
            
            print(instance.transform.position); 

            // Set label according to Data Object


        }

        if(hasFreeTextField)
        {
            Vector3 fieldPos = new Vector3(startPos.x, startPos.y - (toggleNumber * 50), 0);
            
            inputField = Instantiate(Resources.Load<GameObject>("ToggleTextInputObj"), fieldPos, Quaternion.identity );
            inputField.GetComponent<Toggle>().isOn = false;
            inputField.GetComponentInChildren<InputField>().interactable = false; 
            inputField.transform.SetParent(gameObject.transform, false);
            
        }

    }

    public void RemoveToggles()
    {
        int i = 0; 
      
        if(transform.childCount == 0)
        {
            Debug.Log("There are no Toggles to remove!");
            return;
        }

        GameObject[] children = new GameObject[transform.childCount];

        foreach(Transform child in transform)
        {
            children[i] = child.gameObject;
            i++; 
        }

        foreach (GameObject child in children)
            DestroyImmediate(child.gameObject); 
            

    }

    #endregion


}
