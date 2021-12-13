using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SQLSaveManager : MonoBehaviour
{
    public static SQLSaveManager instance;

    [SerializeField] GameObject errorMessageCanvas;

    public enum Group
    {
        group1,
        group2,
        group3
    }
    public Group group;

    public string playerID;
    private string URL = "https://marki.fun/PHP/dataNew.php";

    public struct Answer
    {
        public string name;
        public string answer;
    }
    public List<Answer> answerList = new List<Answer>();

    public List<NBackGameManager.LevelData> nBackData = new List<NBackGameManager.LevelData>();
    public SaveTrialClass[] primaryData = new SaveTrialClass[0];

    string startTime;
    string uploadTime; 




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

        startTime = System.DateTime.UtcNow.ToString(); 
        print(startTime); 

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
        print("Answer: " + tempAnswer.name + " with the answer: " + tempAnswer.answer + " has been added to the list.");
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
        uploadTime = System.DateTime.UtcNow.ToString();

        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();

        formData.Add(new MultipartFormDataSection("id", playerID));
        formData.Add(new MultipartFormDataSection("startTime", startTime));
        formData.Add(new MultipartFormDataSection("uploadTime", uploadTime));

        formData.Add(new MultipartFormDataSection("gr", group.ToString()));

        foreach (Answer a in answerList)
        {
            formData.Add(new MultipartFormDataSection(a.name, a.answer));
        }

        if (nBackData != null)
        {
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
        }
        else
            Debug.LogError("No NBack Data Found!");


        primaryData = GetComponents<SaveTrialClass>(); 
        if(primaryData != null)
        {
            foreach(SaveTrialClass data in primaryData)
            {
                //formData.Add(new MultipartFormDataSection("trialName", data.trialName));
                formData.Add(new MultipartFormDataSection(data.trialName + "_score", data.score.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_carsTotal", data.carsTotal.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_carsSuccess", data.carsSuccess.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_carsCrashed", data.carsCrashed.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_crossesCrossed", data.crossesCrossed.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_assistance", data.assistance));
                formData.Add(new MultipartFormDataSection(data.trialName + "_changedAssistanceAmount", data.changedAssistanceAmount.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_percentageArea", data.percentageArea.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_percentageSpecific", data.percentageSpecific.ToString()));

                formData.Add(new MultipartFormDataSection(data.trialName + "_sec_TotalAlarms", data.sec_TotalAlarms.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_sec_Correct", data.sec_Correct.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_sec_Misses", data.sec_Misses.ToString()));
                formData.Add(new MultipartFormDataSection(data.trialName + "_sec_FalseAlarm", data.sec_FalseAlarm.ToString()));



            }
        }
        else
            Debug.LogError("No PrimaryTask Data Found!");


        using (UnityWebRequest webRequest = UnityWebRequest.Post(URL, formData))
        {
            yield return webRequest.SendWebRequest();

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    StartCoroutine(retrySendData());
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    StartCoroutine(retrySendData());
                    Debug.LogError("PostDataRequest Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    StartCoroutine(retrySendData());
                    Debug.LogError("PostDataRequest HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log("PostDataRequest Response: " + webRequest.downloadHandler.text);
                    break;
            }
        }

        IEnumerator retrySendData ()
        {
            GameObject tempError = Instantiate(errorMessageCanvas);
            yield return new WaitForSeconds(1);
            Destroy(tempError);
            StartCoroutine(PostData());
        }
    }

   
}
