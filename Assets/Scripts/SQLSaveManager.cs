using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class SQLSaveManager : MonoBehaviour
{
    public static SQLSaveManager instance; 

    public string playerID; 
    string URL; 


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

        //Create Unique Player ID ?
        //playerID = SystemInfo.deviceUniqueIdentifier; 
        playerID = "simon_test"; 
        URL = "http://localhost/sqlCarControl/exDataManager.php"; 
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
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post("http://127.0.0.1:8888/data.php", formData))
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
