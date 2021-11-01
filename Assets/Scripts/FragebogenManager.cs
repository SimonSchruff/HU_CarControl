using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 



public class FragebogenManager : MonoBehaviour
{  

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




    public void NextQuestion(int currentID) //Called by Continue Button
    {
        


        if(questions[currentID].questionObj.GetComponent<AnswerSaver>() != null) // Exclude start screen
        {
            AnswerSaver answer = questions[currentID].questionObj.GetComponent<AnswerSaver>(); 
            string name = questions[currentID].name;
            int id = questions[currentID].id; 

            if (answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
            {
            
                if(answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                    return; 
                else
                {   
                    answer.SaveAnswer(id, name);     
                    
                }
            }
            else
            {
                answer.SaveAnswer(id,name);
            }
        }
        

        
        
        //Continue to next question
        questions[currentID].questionObj.gameObject.SetActive(false); 

        if(currentID != (questions.Length - 1) ) // Last question -> Dont activate next UI
            questions[currentID + 1].questionObj.gameObject.SetActive(true); 
        
        
    
        

    }
}
