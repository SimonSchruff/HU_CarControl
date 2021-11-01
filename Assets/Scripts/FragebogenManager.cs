using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 



public class FragebogenManager : MonoBehaviour
{  

    [Header("Events for Certain Question ID`s")]
    public int[] IDToExcludeAnswerSave; 
    public int IDToNBack; 

    // Dictionary not able to be shown in inspector -> Workaround
    [Serializable] 
    public struct Questions
    {
        public int id;
        public string name; 
        public GameObject questionObj;  

    }
    [Header("Questions List")]
    public Questions[] questions;

    public void Awake()
    {
        FragebogenManager.DontDestroyOnLoad(gameObject); 
    }



    public void NextQuestion(int currentID) //Called by Continue Button
    {

        if(currentID != IDToExcludeAnswerSave[0] && currentID != IDToExcludeAnswerSave[1]&& currentID != IDToExcludeAnswerSave[2]) // Exclude start screen
        {
           AnswerSaver answer = questions[currentID].questionObj.GetComponent<AnswerSaver>(); 
           string name = questions[currentID].name; 

            if(answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
            {
            
                if(answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                    return; 
                else
                {   
                    answer.SaveAnswer(currentID, name);     
                    
                }
                    
               
            }
            else
            {
                answer.SaveAnswer(currentID,name); 
                
            }
        }
        

        
        
        //Continue to next question
        questions[currentID].questionObj.gameObject.SetActive(false); 

        if(currentID != IDToNBack)
            questions[currentID + 1].questionObj.gameObject.SetActive(true); 
        
        
    
        

    }
}
