using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DevTools : MonoBehaviour
{


    void Start()
    {
        DontDestroyOnLoad(gameObject); 
    }

    public void SkipNback()
    {
        NBackGameManager.Instance.SaveScoreData();
        MySceneManager.Instance.LoadSceneByName("Fragebogen -Teil 01 b"); 
    }

    public void SkipQuestion()
    {
        FragebogenManager instance = FragebogenManager.fra;
        int id = instance.questionNumber;
        instance.questionNumber = id + 2;
        instance.questions[id].questionObj.SetActive(false);
        instance.questions[id + 2].questionObj.SetActive(true);

    }

    public void SkipTrial(string trialName)
    {
        FragebogenManager instance = FragebogenManager.fra;
        int id = instance.questionNumber; 
        instance.questionNumber = id + 2;
        instance.questions[id].questionObj.SetActive(false);
        instance.questions[id + 2].questionObj.SetActive(true);
        SaveTrialClass saveTrial =  SQLSaveManager.instance.gameObject.AddComponent<SaveTrialClass>();
        saveTrial.trialName = trialName;
        saveTrial.assistance = "skipped"; 

    }
}
