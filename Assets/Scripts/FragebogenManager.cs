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
        public GameObject questionObj;  

    }
    public Questions[] questions;





    public void NextQuestion(int currentID) //Called by Continue Button
    {

        if(currentID > 0) // Exclude start screen
        {
           AnswerSaver answer = questions[currentID].questionObj.GetComponent<AnswerSaver>(); 

            if(answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
            {
            
                if(answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                    return; 
                else
                    answer.SaveAnswer(currentID); 
               
            }
            else
            {
                answer.SaveAnswer(currentID); 
            }
        }
        
        //Continue to next question
        questions[currentID].questionObj.gameObject.SetActive(false); 
        questions[currentID + 1].questionObj.gameObject.SetActive(true); 

    }
}
