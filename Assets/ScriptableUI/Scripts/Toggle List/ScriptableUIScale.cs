using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



[RequireComponent(typeof(ToggleGroup))]
public class ScriptableUIScale : ScriptableUI
{
    //Variables etc
    int toggleNumber;
    string[] texts;
    Vector2 startPos;
    string objName;
    Color color; 
    int spacing; 

    //Obj Ref
    Canvas canvas;
    Toggle[] toggleChildren; 
    InputField inputField; 
    Toggle inputFieldToggle; 
    ToggleGroup toggleGroup; 

    //Bools
    bool hasFreeTextField = false;
    bool vertical; 

    public override void Awake()
    {
        base.Awake(); 


        //Look for all Children Toggles etc.
        if(GetComponentsInChildren<Toggle>() != null) 
        {
            toggleChildren = GetComponentsInChildren<Toggle>(); 
            foreach(Toggle toggle in toggleChildren)
            {
                toggle.isOn = false; 
            }
        }
            

        if(GetComponentInChildren<InputField>() != null)
        {
            inputField = GetComponentInChildren<InputField>(); 
            inputField.interactable = false; 

            inputFieldToggle = inputField.GetComponentInParent<Toggle>(); 
            inputFieldToggle.isOn = false; 
            
            inputFieldToggle.onValueChanged.AddListener(delegate { SetInputFieldInteractable(); }); 
        }
            


    }

    public override void Update()
    {
        base.Update(); 

    }

    public void SetInputFieldInteractable()
    {   // Called from Toggle.OnValueChanged Event in Awake()
        if(inputField != null)
            inputField.interactable = !inputField.interactable;
        else
            Debug.Log("No Input Field Set!"); 
    }




    #region Editor/Inspector Button Events
    public void CreateToggles()
    {
        //Get All variables from ScriptableUIData
        toggleNumber = skinData.toggleData.toggleAmount;
        texts = skinData.toggleData.toggleDescriptions;
        hasFreeTextField = skinData.toggleData.hasFreeTextField;
        startPos = skinData.toggleData.startPos;
        objName = skinData.toggleData.objName;
        color = skinData.toggleData.textColor;
        spacing = skinData.toggleData.spacing; 
        canvas = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Canvas>(); 

        toggleGroup = GetComponent<ToggleGroup>(); 

        
        for (int i = 0; i < toggleNumber; i++)
        {
            
            GameObject instance = Instantiate(Resources.Load<GameObject>(objName));
            instance.name = objName + " " + i.ToString();
            instance.GetComponentInChildren<Text>().text = texts[i];
            instance.GetComponentInChildren<Text>().color = color;
            instance.GetComponent<Toggle>().isOn = false; 
            instance.GetComponent<Toggle>().group = toggleGroup; 
            instance.transform.SetParent(gameObject.transform, false);

            //Spacing als variabel
            //X statt Y Pos verändern wenn bool vertical false



            //problem: takes canvas pos and adds up start pos instead of taking start pos as position
            Vector3 canvasPos = new Vector3(canvas.transform.position.x, canvas.transform.position.y , 0 );
            print(canvasPos);

            instance.transform.localPosition = new Vector3(startPos.x, startPos.y - (i * spacing), 0); 
            
            print(instance.transform.position); 

            // Set label according to Data Object


        }

        if(hasFreeTextField)
        {
            Vector3 fieldPos = new Vector3(startPos.x, startPos.y - (toggleNumber * spacing), 0);
            
            GameObject instance = Instantiate(Resources.Load<GameObject>("ToggleTextInputObj"), fieldPos, Quaternion.identity );
            instance.GetComponent<Toggle>().isOn = false;
            instance.GetComponent<Toggle>().group = toggleGroup; 
            instance.GetComponentInChildren<InputField>().interactable = false; 
            instance.transform.SetParent(gameObject.transform, false);
            
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
