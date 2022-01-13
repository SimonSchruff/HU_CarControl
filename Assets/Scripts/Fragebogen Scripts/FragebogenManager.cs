using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class FragebogenManager : MonoBehaviour
{
    [SerializeField] private int questionNumber; // Counted up with each button "Continue" Button Click
    [SerializeField] private GameObject ineligableScreen;

    [Serializable]
    public struct Questions
    {
        public int id;
        public string name;
        public GameObject questionObj;
    }

    public static FragebogenManager fra;
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

    private void Awake()
    {
        if(fra == null)
        {
            fra = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void NextQuestion() //Called by Continue Button
    {
        bool isAllowedToChange;

        foreach (ErrorText et in errorTexts)
            et.obj.SetActive(false);


        int currentID = questions[questionNumber].id;
        isAllowedToChange = AllowedToContinue(currentID);
        //print(isAllowedToChange);

        
        if (isAllowedToChange)
        {
            SaveAnswer(currentID);

            questions[currentID].questionObj.gameObject.SetActive(false);
            //questions[currentID +1 ].questionObj.gameObject.SetActive(true);
            print(questions[currentID].questionObj.gameObject.name); 

            
            if(questions[currentID].questionObj.gameObject.name ==  "Set Active Self Eff")
            {
                MySceneManager.Instance.LoadSceneByName("NBackSpiel"); 
            }
            else if (questions[currentID].questionObj.gameObject.name == "Set Active Need For Control")
            {
                MySceneManager.Instance.LoadSceneByName("Introduction");
            }
            else if(questions[currentID].questionObj.gameObject.name == "Page 5")
            {
                MySceneManager.Instance.LoadSceneByName("Fragebogen");

            }
            else if(questions[currentID].questionObj.gameObject.name == "Set Active Sat Sliders")
            {
                MySceneManager.Instance.LoadSceneByName("Introduction Part2");
            }
            else if(questions[currentID].questionObj.gameObject.name == "Data Quality")
            {
                SQLSaveManager.instance.StartPostCoroutine();
                questions[currentID + 1].questionObj.gameObject.SetActive(true);
                
            }
            else if (currentID != (questions.Length - 1)) // Last question -> Dont activate next UI
            {
                GameObject obj = questions[currentID + 1].questionObj.gameObject;
                obj.SetActive(true);

                TrialStartLogic tr; 
                if(obj.TryGetComponent<TrialStartLogic>(out tr) == true)
                {
                    tr.ActivateSession();

                }
                
            }
            


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


    public bool AllowedToContinue(int currentID)
    {
        if (questions[currentID].questionObj.gameObject.GetComponentInChildren<AnswerSaver>() != null)
        {
            int answerAmount = 0;
            AnswerSaver[] allAnswers = questions[currentID].questionObj.gameObject.GetComponentsInChildren<AnswerSaver>();
            //print(questions[currentID].questionObj.gameObject.name); 

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
                        else if(answer.gameObject.name == "atc_license" || answer.gameObject.name == "declarationConsent")
                        {
                            if (answer.currentAnswer == "2")
                            {
                                ShowIneligableScreen();
                                return false;
                            }
                        }
                        else if(answer.gameObject.name == "intro1_1" || answer.gameObject.name == "intro1_3" || answer.gameObject.name == "intro2_3")
                        {
                            if (answer.currentAnswer == "2") // Answered 2:False
                                return false; 
                        }
                        else if (answer.gameObject.name == "intro1_2" || answer.gameObject.name == "intro2_1" || answer.gameObject.name == "intro2_2")
                        {
                            if (answer.currentAnswer == "1") // Answered 1:True
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
                            if(answer.currentAnswer.Length != 4)
                            {
                                return false; 
                            }
                    }
                }

                if(answer.questionType == AnswerSaver.QuestionType.other)
                {
                    if(answer.currentAnswer == null || answer.currentAnswer == "")
                    {
                        print("False at free input: " + answer.gameObject.name);
                        return false; 
                    }
                }
            } 
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
            // If education etc. save number AND free text field 
            answer.SaveAnswer(answer.gameObject.name);
        }
    }

    public void ShowIneligableScreen()
    {
        if(ineligableScreen == null)
        {
            Debug.LogError("Ineligable Screen Refernce, not set!");
            return; 
        }

        foreach (ErrorText et in errorTexts)
            et.obj.SetActive(false);

        foreach (Questions q in questions)
            q.questionObj.SetActive(false);

        ineligableScreen.SetActive(true); 


    }

   
}
