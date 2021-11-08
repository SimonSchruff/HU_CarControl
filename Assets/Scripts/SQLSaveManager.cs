using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SQLSaveManager : MonoBehaviour
{
    public static SQLSaveManager instance;

    public string playerID;
    [SerializeField] string URL = "http://127.0.0.1:8888/data.php";

    public struct Answer
    {
        public int id;
        public string name;
        public string answer;
    }
    public List<Answer> answerList = new List<Answer>(); 


    void Awake()
    {
        //Singleton
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        else
            instance = this;


        DontDestroyOnLoad(gameObject);

    }

    public void SaveProlificID(string pID)
    {
        playerID = pID;
    }

    public void AddAnswerToList(int i, string n, string a) // id, name, answer
    {
        Answer tempAnswer = new Answer();
        tempAnswer.id = i;
        tempAnswer.name = n;
        tempAnswer.answer = a; 

        answerList.Add(tempAnswer); 
        
    }

    public void StartAnwerPostCoroutine(Answer[] allAnswers)
    {
        // Start Coroutine to Post all Answers
        // Sameas PostNBackData
        // To different table, or same one ?? 
    }

    public void StartNBackPostCoroutine(NBackGameManager.LevelData data)
    {
        StartCoroutine(PostNBackData(data)); 
    }


    IEnumerator PostNBackData(NBackGameManager.LevelData data)
    {
        #region Add Data to Form
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>(); 

        // DATA
        formData.Add(new MultipartFormDataSection("id", playerID)); 
        formData.Add(new MultipartFormDataSection("currentLevel", data.currentLevel.ToString())); 

        // EVENTS IN TOTAL NUMBERS
        formData.Add(new MultipartFormDataSection("totalCorrectMatches", data.totalCorrectMatches.ToString())); 
        formData.Add(new MultipartFormDataSection("totalCorrectMismatches", data.totalCorrectMismatches.ToString())); 

        formData.Add(new MultipartFormDataSection("totalFalseDecisionMatch", data.totalFalseDecisionMatch.ToString())); 
        formData.Add(new MultipartFormDataSection("totalFalseDecisionMismatch", data.totalFalseDecisionMismatch.ToString())); 

        formData.Add(new MultipartFormDataSection("totalNoReactionMatches", data.totalNoReactionMatches.ToString())); 
        formData.Add(new MultipartFormDataSection("totalNoReactionMismatches", data.totalNoReactionMismatches.ToString())); 

        // EVENTS IN PERCENTAGES
        formData.Add(new MultipartFormDataSection("totalCorrectPercentage", data.totalCorrectPercentage.ToString())); 
        formData.Add(new MultipartFormDataSection("correctlyMatched", data.correctlyMatched.ToString())); 
        formData.Add(new MultipartFormDataSection("correctlyMismatched", data.correctlyMismatched.ToString())); 

        formData.Add(new MultipartFormDataSection("noReactionMatches", data.noReactionMatches.ToString())); 
        formData.Add(new MultipartFormDataSection("noReactionMismatches", data.noReactionMismatches.ToString())); 

        formData.Add(new MultipartFormDataSection("falseDecisionMatch", data.falseDecisionMatch.ToString())); 
        formData.Add(new MultipartFormDataSection("falseDecisionMismatch", data.falseDecisionMismatch.ToString())); 
        #endregion
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData))
        {
            yield return webRequest.SendWebRequest(); 

            switch(webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("PostNBackDataRequest Error: " + webRequest.error);
                break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("PostNBackDataRequest HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("PostNBackDataRequest Response: " + webRequest.downloadHandler.text);

                break;
            }
        } 
    }


}
