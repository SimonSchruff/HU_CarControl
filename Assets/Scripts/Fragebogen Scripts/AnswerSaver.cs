using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnswerSaver : MonoBehaviour
{

    public string currentAnswer;
    public InputField toggleIF; 

    Toggle[] toggles; 
    InputField inputField;
    Slider slider; 

    
    public enum QuestionType
    {
        toggles, 
        togglesWithFreeInput, 
        freeInputNumber,
        freeInputAlphaNum,
        slider,
        other
    }
    [Tooltip("Set Type of Question to save Answer")]
    public QuestionType questionType ;

    void Awake()
    {
        switch(questionType)
        {
            case QuestionType.toggles:
                toggles = GetComponentsInChildren<Toggle>();
                break;
            case QuestionType.togglesWithFreeInput:
                toggles = GetComponentsInChildren<Toggle>();
                inputField = GetComponentInChildren<InputField>(); 
                break;
            case QuestionType.freeInputNumber:
                inputField = GetComponentInChildren<InputField>();
                break;
            case QuestionType.slider:
                slider = GetComponentInChildren<Slider>();
                break;
            case QuestionType.freeInputAlphaNum:
                inputField = GetComponentInChildren<InputField>();
                break;
            case QuestionType.other:
                currentAnswer = "null"; 
                break;
        }
    }

    void Update()
    {
        switch(questionType)
        {
            case QuestionType.toggles: 

                foreach(Toggle toggle in toggles)
                {
                    //print(toggle.gameObject.name); 
                    
                    if(toggle.isOn == true)
                    {
                        currentAnswer = toggle.GetComponentInChildren<Text>().text;
                        if(name == "education" && currentAnswer == "8" || name == "countryOfResidence" && currentAnswer == "9" || name == "employment" && currentAnswer == "10" )
                        {
                            toggleIF.GetComponentInParent<AnswerSaver>().currentAnswer = toggleIF.text; 
                        }
                    }
                }
                break; 
            case QuestionType.togglesWithFreeInput: 

                foreach(Toggle toggle in toggles)
                {
                    if(toggle.isOn == true)
                    {
                        if(toggle.GetComponentInChildren<InputField>() != null)
                        {
                            inputField.interactable = true; 
                            currentAnswer = inputField.text; 
                        }
                        else
                        {
                            inputField.interactable = false;
                            currentAnswer = toggle.GetComponentInChildren<Text>().text; 
                        }
                        
                    }
                }

                break;
            case QuestionType.freeInputNumber: 
                currentAnswer = inputField.text; 
                break;
            case QuestionType.slider:
                currentAnswer = slider.value.ToString(); 
                break;
            case QuestionType.freeInputAlphaNum:
                currentAnswer = inputField.text;
                break;
            case QuestionType.other:
                break;
        }
    }


    public void SaveAnswer(string name)
    {
        //Save to SQL Database with SQL Save Manager        
        if(name == "prolificID")
        {
            SQLSaveManager.instance.SaveProlificID(currentAnswer); 
        }
        else if(name == "intro1_1" || name == "intro1_2" || name == "intro1_3" || name == "intro2_1" || name == "intro2_2" || name == "intro2_3") // Do not need to be saved; 
        {
            return;
        }
        else
        {
            //SQLSaveManager.instance.AddAnswerToList(questionID, name, currentAnswer);
            SQLSaveManager.instance.AddAnswerToList(name, currentAnswer);  
        }


        /*
         * else if(name == "education" || name == "countryOfResidence" || name == "employment" )
        {
            if(toggleIF != null)
            {
                if(name == "education")
                {
                    GameObject other = GameObject.Find("education_other");
                    if (other == null)
                        Debug.LogError("Game Object education_other not Found!");
                    else
                    {
                        string otherAnswer = other.GetComponent<AnswerSaver>().currentAnswer = toggleIF.text;
                    }
                    
                }
                else if (name == "countryOfResidence")
                {
                    GameObject other = GameObject.Find("countryOfResidence_other");
                    if (other == null)
                        Debug.LogError("Game Object countryOfResidence_other not Found!");
                    else
                    {
                        string otherAnswer = other.GetComponent<AnswerSaver>().currentAnswer = toggleIF.text;
                    }
                }
                else if (name == "employment")
                {
                    GameObject other = GameObject.Find("employment_other");
                    if (other == null)
                        Debug.LogError("Game Object employment_other not Found!");
                    else
                    {
                        string otherAnswer = other.GetComponent<AnswerSaver>().currentAnswer = toggleIF.text;
                    }
                }
            }
            else
            {
                Debug.LogError("Input Field for Education/Country/Employment Not set!"); 
            }
        }
         */

    }
}
