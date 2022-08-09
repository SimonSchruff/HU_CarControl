using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SaveTrialClass : MonoBehaviour
{
    public string trialName = "";
    public int score = 0;
    
    [Header("All Cars")]
    public int carsTotal = 0;
    [FormerlySerializedAs("carsSuccess")] public int carsSuccessTotal = 0;
    [FormerlySerializedAs("carsCrashed")] public int carsCrashedTotal = 0;
    
    [Header("Car00")]
    public int cars00Total = 0;
    public int cars00Success = 0;
    public int cars00Crashed = 0;
    [Header("Car01")]
    public int cars01Total = 0;
    public int cars01Success = 0;
    public int cars01Crashed = 0;
    [Header("Car02")]
    public int cars02Total = 0;
    public int cars02Success = 0;
    public int cars02Crashed = 0;
    
    [Header("Cross Variables")]
    public int crossesCrossed = 0;
    public int crossesCrossedHorizontal = 0;
    public int crossesCrossedVertical = 0;
    public int amountUserClickedCrosses = 0;

    [Header("Assistance Variables")]
    public string assistance = "";
    public int changedAssistanceAmount = 0;
    public float percentageArea = 0;
    public float percentageSpecific = 0;
    
    [Header("Secondary Task Variables")]
    public float sec_TotalAlarms = 0;
    public float sec_Correct = 0;
    public float sec_Misses = 0;
    public float sec_FalseAlarm = 0;
    

    private float lastChangeTime;
    private bool isChangeAssistance = false;
    private bool isArea = true;
    float areaTime = 0;
    float specificTime = 0;

    public bool saveData = true;

    bool finished = false;
    
    public void InitChangeAssistanceActive(bool IsArea)
    {
        lastChangeTime = Time.realtimeSinceStartup;
        isChangeAssistance = true;
        isArea = IsArea;
    }
    public void ChangeAssitanceInGame (bool ToArea)
    {
        if (!finished)
        {
            if(isArea != ToArea)    //Add Times to counters
            {
                changedAssistanceAmount++;

                if (ToArea)
                {
                    specificTime += (Time.realtimeSinceStartup - lastChangeTime);
                }
                else
                {
                    areaTime += (Time.realtimeSinceStartup - lastChangeTime);
                }
                isArea = ToArea;
                lastChangeTime = Time.realtimeSinceStartup;
            }
        }
    }

    public void FinishTrial ()
    {
        finished = true;

        if (isChangeAssistance) //Add last time to timeCounters
        {
            if(isArea)
            {
                areaTime += (Time.realtimeSinceStartup - lastChangeTime);
            }
            else
            {
                specificTime += (Time.realtimeSinceStartup - lastChangeTime);
            }
            lastChangeTime = 0;
            float totalTime = specificTime + areaTime;
            percentageArea = areaTime / totalTime;
            percentageSpecific = specificTime / totalTime;
        }

        if (!saveData)
        {
            DestroyImmediate(this);
        }
    }
}
