using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnswerSaver : MonoBehaviour
{

    public string currentAnswer; 

    Toggle[] toggles; 
    InputField inputField;
    Slider slider; 

    
    public enum QuestionType
    {
        toggles, 
        togglesWithFreeInput, 
        freeInputNumber,
        freeInputAlphaNum,
        slider
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
        }
    }

    void Update()
    {
        switch(questionType)
        {
            case QuestionType.toggles: 

                foreach(Toggle toggle in toggles)
                {
                    if(toggle.isOn == true)
                    {
                        currentAnswer = toggle.GetComponentInChildren<Text>().text; 
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
        }
    }


    public void SaveAnswer(int questionID, string name)
    {
        SQLSaveManager saveManager = SQLSaveManager.instance;

        //Save to SQL Database with SQL Save Manager        
        if(questionID == 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Fragebogen")
        {
            // Prolific ID
            saveManager.SaveProlificID(currentAnswer); 
        }
        else
        {
            //SQLSaveManager.instance.AddAnswerToList(questionID, name, currentAnswer);
            saveManager.AddAnswerToList(questionID, name, currentAnswer); 
        }

        print("The question : " + name + ", ID : " + questionID + " with the answer " + currentAnswer + " has been saved!");
    }
}
