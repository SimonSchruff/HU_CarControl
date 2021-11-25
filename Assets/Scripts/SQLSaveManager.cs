using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SQLSaveManager : MonoBehaviour
{
    public static SQLSaveManager instance;

    public string playerID;
    [SerializeField] string URL; 

    public struct Answer
    {
        public string name;
        public string answer;
    }
    public List<Answer> answerList = new List<Answer>();

    public List<NBackGameManager.LevelData> nBackData = new List<NBackGameManager.LevelData>();


    public string testDataString; 

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

    public void AddAnswerToList(string n, string a) // name, answer
    {
        Answer tempAnswer = new Answer();
        tempAnswer.name = n;
        tempAnswer.answer = a;

        answerList.Add(tempAnswer);
        //print("Answer: " + tempAnswer.name + " with the answer: " + tempAnswer.answer + " has been added to the list.");
    }

    public void SaveNBackData(NBackGameManager.LevelData data)
    {
        nBackData.Add(data);
    }

    public void StartPostCoroutine()
    {
        StartCoroutine(PostData());
    }

    IEnumerator PostData()
    {
        print("Posting " + (answerList.Count + 53 ) + " items...");

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("id", playerID));

        foreach(Answer a in answerList)
        {
            formData.Add(new MultipartFormDataSection(a.name, a.answer));
        }
        
        foreach (NBackGameManager.LevelData data in nBackData)
        {
            // EVENTS IN TOTAL NUMBERS
            formData.Add(new MultipartFormDataSection("totalCorrectMatches_" + data.currentLevel.ToString(), data.totalCorrectMatches.ToString()));
            formData.Add(new MultipartFormDataSection("totalCorrectMismatches_" + data.currentLevel.ToString(), data.totalCorrectMismatches.ToString()));

            formData.Add(new MultipartFormDataSection("totalFalseDecisionMatch_" + data.currentLevel.ToString(), data.totalFalseDecisionMatch.ToString()));
            formData.Add(new MultipartFormDataSection("totalFalseDecisionMismatch_" + data.currentLevel.ToString(), data.totalFalseDecisionMismatch.ToString()));

            formData.Add(new MultipartFormDataSection("totalNoReactionMatches_" + data.currentLevel.ToString(), data.totalNoReactionMatches.ToString()));
            formData.Add(new MultipartFormDataSection("totalNoReactionMismatches_" + data.currentLevel.ToString(), data.totalNoReactionMismatches.ToString()));

            // EVENTS IN PERCENTAGES
            formData.Add(new MultipartFormDataSection("totalCorrectPercentage_" + data.currentLevel.ToString(), ((float)data.totalCorrectPercentage).ToString()));
            formData.Add(new MultipartFormDataSection("totalCorrectPercentage_" + data.currentLevel.ToString(), ((float)data.totalCorrectPercentage).ToString()));
            formData.Add(new MultipartFormDataSection("correctlyMatched_" + data.currentLevel.ToString(), ((float)data.correctlyMatched).ToString()));
            formData.Add(new MultipartFormDataSection("correctlyMismatched_" + data.currentLevel.ToString(), ((float)data.correctlyMismatched).ToString()));

            formData.Add(new MultipartFormDataSection("noReactionMatches_" + data.currentLevel.ToString(), ((float)data.noReactionMatches).ToString()));
            formData.Add(new MultipartFormDataSection("noReactionMismatches_" + data.currentLevel.ToString(), ((float)data.noReactionMismatches).ToString()));

            formData.Add(new MultipartFormDataSection("falseDecisionMatch_" + data.currentLevel.ToString(), ((float)data.falseDecisionMatch).ToString()));
            formData.Add(new MultipartFormDataSection("falseDecisionMismatch_" + data.currentLevel.ToString(), ((float)data.falseDecisionMismatch).ToString()));
        }
        

        using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("PostDataRequest Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("PostDataRequest HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("PostDataRequest Response: " + webRequest.downloadHandler.text);

                    break;
            }
        }
    }

   
}
