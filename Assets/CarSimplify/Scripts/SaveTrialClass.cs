using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrialClass : MonoBehaviour
{
    public string trialName = "";
    public int score = 0;
    public int carsTotal = 0;
    public int carsSuccess = 0;
    public int carsCrashed = 0;
    public int crossesCrossed = 0;
    public string assistance = "";
    public int changedAssistanceAmount = 0;
    public float percentageArea = 0;
    public float percentageSpecific = 0;

    private float lastChangeTime;
    private bool isChangeAssistance = false;
    private bool isArea = true;
    float areaTime = 0;
    float specificTime = 0;

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
    }
}
