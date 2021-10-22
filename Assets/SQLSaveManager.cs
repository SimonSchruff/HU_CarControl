using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public class SQLSaveManager : MonoBehaviour
{
    public static SQLSaveManager instance; 

    public string playerID; 


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
        playerID = SystemInfo.deviceUniqueIdentifier; 
    }

    void Update()
    {
        
    }

    public void StartNBackPostCoroutine(NBackGameManager.LevelData data)
    {
        StartCoroutine(PostNbackLevelData(data)); 
    }


    IEnumerator PostNbackLevelData(NBackGameManager.LevelData data)
    {
        
        string dataString = JsonUtility.ToJson(data); 
        Debug.Log(dataString); 
        
        // Updated to IMulitpartFormSection
        WWWForm postForm = new WWWForm(); 
        postForm.AddField("id", playerID); 

        postForm.AddField("level", data.level.ToString()); 
        postForm.AddField("total Matches", data.totalMatches.ToString()); 
        postForm.AddField("total Mismatches", data.totalMismatches.ToString()); 

        postForm.AddField("correctly Matched", data.correctlyMatched.ToString()); 
        postForm.AddField("false Alarm", data.falseAlarm.ToString()); 
        postForm.AddField("missed Matches", data.missedMatches.ToString()); 

        postForm.AddField("correctly Mismatched", data.correctlyMismatched.ToString()); 
        postForm.AddField("false Mismatch Alarm", data.falseAlarmMismatch.ToString()); 
        postForm.AddField("missed Mismatches", data.missedMismatches.ToString());
        
        using (UnityWebRequest webRequest = UnityWebRequest.Post("LINK", postForm))
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
