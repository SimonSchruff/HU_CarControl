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
    [Header("Questions List")]
    public Questions[] questions;


    public GameObject notAllAnswersGivenText;

    #region Continue Button Event

    /*
     * After each Continue Button Click questionNumber/randId is counted up 
     * Disable current Question and enable next question
     * Save Answer if valid
     */


    /*
     * TO DO: 
     * Get All Answers if multiple Questions on Page
     * Rebuild Next Question Method to work with multiple AnswerSavers on Page; 
     */


    public void NextQuestion() //Called by Continue Button
    {
        bool isAllowedToChange;
        notAllAnswersGivenText.SetActive(false);

        int currentID = questions[questionNumber].id;
        isAllowedToChange = AllowedToContinue(currentID);
        print(isAllowedToChange); 
        
        if (isAllowedToChange)
        {
            SaveAnswer(currentID); 

            questions[currentID].questionObj.gameObject.SetActive(false);

            if (currentID != (questions.Length - 1 )) // Last question -> Dont activate next UI
                questions[currentID + 1].questionObj.gameObject.SetActive(true);


            questionNumber++;
        }
        else
        {
            notAllAnswersGivenText.SetActive(true); 
        }
                

        
    }

    #endregion
    bool AllowedToContinue(int currentID)
    {
        if (questions[currentID].questionObj.gameObject.GetComponentInChildren<AnswerSaver>() != null)
        {
            int answerAmount = 0; 
            AnswerSaver[] allAnswers = questions[currentID].questionObj.gameObject.GetComponentsInChildren<AnswerSaver>();
            foreach (AnswerSaver answer in allAnswers)
            {


                if (answer.questionType == AnswerSaver.QuestionType.toggles || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
                {
                    if (answer.GetComponentInChildren<ToggleGroup>() != null)
                    {
                        ToggleGroup tg = answer.gameObject.GetComponentInChildren<ToggleGroup>();
                        if (tg.AnyTogglesOn() == false)
                        {
                            print("No Toggle is turned on; ");
                            return false;
                        }
                        else
                        {
                            answerAmount++;
                            if (answerAmount == allAnswers.Length)
                                return true;
                        }
                    }
                }



                if (answer.questionType == AnswerSaver.QuestionType.freeInputAlphaNum || answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
                {
                    if (answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                    {
                        return false;
                    }

                    if (answer.gameObject.name == "Prolific ID") // Prolific ID
                    {
                        bool returnBool;
                        returnBool = CheckStringForLength(24, answer);

                        if (!returnBool) // String not long enough
                        {
                            return returnBool;
                        }
                        else
                        {
                            return returnBool;
                        }

                    }
                    else if (answer.gameObject.name == "Frage 02 - Age") // Age
                    {
                        int i = int.Parse(answer.currentAnswer);
                        if (i < 18 || i > 99) // Age not valid
                        {
                            return false;
                        }
                        else
                        {
                            answerAmount++;
                            if (answerAmount == allAnswers.Length)
                                return true;
                        }

                    }
                    else
                    {
                        answerAmount++;
                        if (answerAmount == allAnswers.Length)
                            return true;
                    }
                }
                
            } // Foreach Bracket

            print("Mystery Code Path");
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
        foreach(AnswerSaver answer in allAnswers)
        {
            answer.SaveAnswer(currentID, answer.gameObject.name);
        }
    }

    bool SaveCurrentAnswer(int currentID)
    {

        if (questions[currentID].questionObj.gameObject.GetComponentInChildren<AnswerSaver>() != null) // Exclude start/instructions screen
        {
            // Save Answer Saver Array and check for length
            // If length == 1 save as normal
            // If length > 1 save individually

            // Get all Anser Saver Objects in active question OBJ
            AnswerSaver[] allAnswers = questions[currentID].questionObj.gameObject.GetComponentsInChildren<AnswerSaver>();
            print(allAnswers.Length); 


            foreach (AnswerSaver answer in allAnswers)
            {
                //AnswerSaver answer = questions[currentID].questionObj.GetComponent<AnswerSaver>();
                string name = questions[currentID].name;
                int id = questions[currentID].id;


                #region If no Answer return false otherwise true


                if (answer.questionType == AnswerSaver.QuestionType.toggles || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
                {
                    if (answer.GetComponentInChildren<ToggleGroup>() != null)
                    {
                        ToggleGroup tg = answer.gameObject.GetComponentInChildren<ToggleGroup>();
                        if (tg.AnyTogglesOn() == false)
                        {
                            print("No Toggle is turned on; ");
                            questionNumber--;
                            return false;
                        }
                    }
                }

                if (answer.questionType == AnswerSaver.QuestionType.freeInputAlphaNum || answer.questionType == AnswerSaver.QuestionType.freeInputNumber || answer.questionType == AnswerSaver.QuestionType.togglesWithFreeInput)
                {
                    if (answer.currentAnswer == null || answer.currentAnswer == "") // If no input happened in free input field dont allow continue
                    {
                        questionNumber--;
                        return false;
                    }
                    else //Answer == free Input Field
                    {


                        if (id == 0 && SceneManager.GetActiveScene().name == "Fragebogen") // Prolific ID
                        {
                            bool returnBool;
                            returnBool = CheckStringForLength(24, answer);

                            if (!returnBool) // String not long enough
                            {
                                questionNumber--;
                                return returnBool;
                            }
                            else
                            {
                                answer.SaveAnswer(id, name);
                                return returnBool;
                            }

                        }
                        else if (id == 3 && SceneManager.GetActiveScene().name == "Fragebogen") // Age
                        {
                            int i = int.Parse(answer.currentAnswer);
                            if (i < 18 || i > 99) // Age not valid
                            {
                                questionNumber--;
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


                }
                else
                {
                    answer.SaveAnswer(id, answer.gameObject.name);
                    return true;

                }


            } // Foreach Bracket

                
                return true; // WHY DO I NEED THIS ? Otherwise Error : Not all code paths return value

            #endregion
        }
        else // Anser Saver == null
        {

            return true;
        }


    } // Save Anser Bracket



    bool CheckStringForLength(int length, AnswerSaver a )
    {
        if(a.currentAnswer.Length == length )
        {
            return true; 
        }
        else
        {
            return false; 
        }
    }





    // Code used for Randomizing Questions; Currently not used; (Saved for maybe maybe) 

    /*
    void RandomizeQuestions(Questions[] q)
    {
        // Shuffle List of Questions
        // Position in Array is shuffled but NOT id of question

        for (int i = 0; i < q.Length; i++)
        {
            Questions temp = q[i];
            int randomIndex = UnityEngine.Random.Range(i, q.Length);
            q[i] = q[randomIndex];
            q[randomIndex] = temp;
        }
    }

     public void NextRandomOrderQuestion()
    {
        bool isAllowedToChange;

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

    */
}
