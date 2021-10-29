using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AnswerSaver : MonoBehaviour
{

    public string currentAnswer; 

    Toggle[] toggles; 
    InputField inputField; 


    public enum QuestionType
    {
        toggles, 
        togglesWithFreeInput, 
        freeInputNumber
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
        else if(GetComponentInChildren<ToggleGroup>() == null || GetComponentInChildren<InputField>() != null)
        { // Input Field for Numbers
            
            inputField = GetComponentInChildren<InputField>();
            if(inputField.contentType == InputField.ContentType.IntegerNumber)
            {
                questionType = QuestionType.freeInputNumber; 
            }
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
        }
    }


    public void SaveAnswer(int questionID)
    {
        print(currentAnswer + " with the ID " + questionID + " has been saved!"); 
        //Save to SQL Database with SQL Save Manager
    }
}
