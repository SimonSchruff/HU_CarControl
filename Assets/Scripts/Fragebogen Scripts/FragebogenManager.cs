using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class FragebogenManager : MonoBehaviour
{
    [SerializeField] int questionNumber; // Counted up with each button "Continue" Button Click

    // Dictionary not able to be shown in inspector -> Workaround
    [Serializable]
    public struct Questions
    {
        public int id;
        public string name;
        public GameObject questionObj;

    }
    public Questions[] questions;

    [Serializable]
    public struct ErrorText
    {
        public string parentObjectName;
        public GameObject obj;
    }
    [Tooltip("Set the parent objects name of the Question Obj and the corresponding Error Text Game Object")]
    public ErrorText[] errorTexts;

    /*
     * 
     * If all Answers have been given / valid -> Disable current Question and enable next question
     * Save Answer 
     * Count up Question ID
     * Enable Error Text if not all answers have been given / valid 
     * 
     */


    public void NextQuestion() //Called by Continue Button
    {
        bool isAllowedToChange;

        foreach (ErrorText et in errorTexts)
            et.obj.SetActive(false);


        int currentID = questions[questionNumber].id;
        isAllowedToChange = AllowedToContinue(currentID);
        print(isAllowedToChange);

        if (isAllowedToChange)
        {
            SaveAnswer(currentID);

            questions[currentID].questionObj.gameObject.SetActive(false);

            if (currentID != (questions.Length - 1)) // Last question -> Dont activate next UI
                questions[currentID + 1].questionObj.gameObject.SetActive(true);


            questionNumber++;
        }
        else
        {
            //print(questions[currentID].questionObj.gameObject.name);

            //Displays Error Text if not all Answers have been answered
            foreach (ErrorText t in errorTexts)
            {
                if (questions[currentID].questionObj.gameObject.name == t.parentObjectName)
                    t.obj.SetActive(true);
            }

        }
    }


    bool AllowedToContinue(int currentID)
    {
        if (questions[currentID].questionObj.gameObject.GetComponentInChildren<AnswerSaver>() != null)
        {
            int answerAmount = 0;
            AnswerSaver[] allAnswers = questions[currentID].questionObj.gameObject.GetComponentsInChildren<AnswerSaver>();
            print(questions[currentID].questionObj.gameObject.name); 

            foreach (AnswerSaver answer in allAnswers)
            {
                answerAmount++; 

                if (answer.questionType == AnswerSaver.QuestionType.toggles || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
                {
                    if (answer.gameObject.GetComponentInChildren<ToggleGroup>() != null)
                    {
                        ToggleGroup tg = answer.gameObject.GetComponentInChildren<ToggleGroup>(); 
                        if (tg.AnyTogglesOn() == false)
                        {
                            print("False at toogle: " + answer.gameObject.name); 
                            return false;
                        }
                        
                    }
                }

                if (answer.questionType == AnswerSaver.QuestionType.freeInputAlphaNum || answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
                {
                    if (answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                    {
                        print("False at free input: " + answer.gameObject.name); 
                        return false;
                    }

                    if(answer.gameObject.name == "prolificID")
                    {
                            bool returnBool;
                            returnBool = CheckStringForLength(24, answer);

                            if (!returnBool) // String not long enough
                            {
                                print("False at prolific; "); 
                                return returnBool;
                            }
                    }
                    else if(answer.gameObject.name == "age")
                    {
                            int i = int.Parse(answer.currentAnswer);
                            if (i < 18 || i > 99) // Age not valid
                            {
                                print("False at age");
                                return false;
                            }
                    }
                } // Question Type Bracket
            } // Foreach Loop Bracket

            // If code reaches this point, each question has been checked for valid answer
            if (answerAmount == allAnswers.Length)
                return true;
            else
                return false;
            
        }     
        else // IntroductionScreens with no Answer Saver attached; 
        {
            return true;
        }
    }

    void SaveAnswer(int currentID)
    {
        AnswerSaver[] allAnswers = questions[currentID].questionObj.gameObject.GetComponentsInChildren<AnswerSaver>();
        foreach (AnswerSaver answer in allAnswers)
        {
            answer.SaveAnswer(currentID, answer.gameObject.name);
        }
    }

    bool CheckStringForLength(int length, AnswerSaver a)
    {
        if (a.currentAnswer.Length == length)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
