using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 



public class FragebogenManager : MonoBehaviour
{
    [SerializeField] int questionNumber; // Counted up with each button "Continue" Button Click
    [SerializeField] int randID = 0; // Seperate Int for random Questions to go through shuffled list

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

    public Questions[] randomOrderQuestions;


    private void Start()
    {
        if(randomOrderQuestions != null)
        {
            RandomizeQuestions( randomOrderQuestions );
        }
    }

    void RandomizeQuestions(Questions[] q)
    {
        // Shuffle List of Questions
        // Position in Array is shuffled but NOT id of question
        for(int i = 0; i < q.Length; i++) {
            Questions temp = q[i];
            int randomIndex = UnityEngine.Random.Range(i, q.Length);
            q[i] = q[randomIndex];



            q[randomIndex] = temp;
        }
    }


    #region Continue Button Event

    /*
     * After each Continue Button Click questionNumber/randId is counted up 
     * Disable current Question and enable next question
     * Save Answer if valid
     */

    public void NextRandomOrderQuestion()
    {
        bool isAllowedToChange;

        questionNumber++;
        randID++;

        int currentID = randID - 1;
        isAllowedToChange = SaveCurrentAnswer(currentID, randomOrderQuestions);

        if(isAllowedToChange)
        {
            randomOrderQuestions[currentID].questionObj.gameObject.SetActive(false);

            if (randID != (randomOrderQuestions.Length)) 
                randomOrderQuestions[currentID + 1].questionObj.gameObject.SetActive(true);
            else // Load new Scene if last question
                MySceneManager.Instance.LoadSceneInt(SceneManager.GetActiveScene().buildIndex + 1 ); 
        }
        

    }


    public void NextQuestion(bool isNextQuestionRandom) //Called by Continue Button
    {
        bool isAllowedToChange;
        questionNumber++;
        

        int currentID = questions[questionNumber - 1].id;
        isAllowedToChange = SaveCurrentAnswer(currentID, questions);
        

        if (isAllowedToChange)
        {
            questions[currentID].questionObj.gameObject.SetActive(false);

            if(isNextQuestionRandom)
            {
                randomOrderQuestions[randID].questionObj.gameObject.SetActive(true);
            }

            if (currentID != (questions.Length - 1 )) // Last question -> Dont activate next UI
                questions[currentID + 1].questionObj.gameObject.SetActive(true);
        }
    }

    #endregion


    bool SaveCurrentAnswer(int currentID, Questions[] list)
    {

        if (list[currentID].questionObj.GetComponent<AnswerSaver>() != null) // Exclude start/instructions screen
        {
            AnswerSaver answer = list[currentID].questionObj.GetComponent<AnswerSaver>();
            string name = list[currentID].name;
            int id = list[currentID].id;
            
            if (answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
            {
                if (answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                {
                    // Count Back Index from list, because counted up through button, but no change to next question
                    if (list == questions)
                    {
                        questionNumber--;
                    }
                    else if (list == randomOrderQuestions)
                    {
                        randID--;
                        questionNumber--;
                    }
                    return false;
                }
                else
                {
                    answer.SaveAnswer(id, name);
                    return true; 
                    
                }
            }
            else
            {
                answer.SaveAnswer(id, name);
                return true; 
                
            }
        }
        else
        {
            return true; 
        }
    }
}
