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

    public QuestionType questionType ;

    void Awake()
    {
        //Decide what type of Question and set enum "QuestionType" Accordingly

        if(GetComponentInChildren<ToggleGroup>() != null)
        {
            toggles = GetComponentsInChildren<Toggle>(); 

            if(GetComponentInChildren<InputField>() != null)
            {
                inputField = GetComponentInChildren<InputField>();
                questionType = QuestionType.togglesWithFreeInput; 
 
            }
            else
                questionType = QuestionType.toggles; 
        }
        else if(GetComponentInChildren<ToggleGroup>() == null && GetComponentInChildren<InputField>() != null)
        { // Input Field for Numbers
            
            inputField = GetComponentInChildren<InputField>();
            if(inputField.contentType == InputField.ContentType.IntegerNumber)
            {
                questionType = QuestionType.freeInputNumber; 
            }
            else if(inputField.contentType == InputField.ContentType.Alphanumeric)
            {
                questionType = QuestionType.freeInputAlphaNum; 
            }
        }
        else if(GetComponentInChildren<Slider>() != null)
        {

            slider = GetComponentInChildren<Slider>();
            questionType = QuestionType.slider;

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
                            currentAnswer = inputField.text; 
                        }
                        else
                        {
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
        if(questionID == 0 )
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
