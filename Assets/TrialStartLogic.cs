using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrialStartLogic : MonoBehaviour
{
    string trialName;
    [SerializeField] bool showUI = true;
    [SerializeField] bool saveData = true;
    [SerializeField] float trialDurationInMin = .5f;
    [SerializeField] bool differentGroupAssistances = false;
    [Header("When differentGroupAssistance = false")]
    [SerializeField] AssistanceSelectScript.AssiSelectStates assistanceLevel;
    [Header("When differentGroupAssistance = true")]
    [SerializeField] AssistanceSelectScript.AssiSelectStates group1_assiLevel;
    [SerializeField] AssistanceSelectScript.AssiSelectStates group2_assiLevel;
    [SerializeField] AssistanceSelectScript.AssiSelectStates group3_assiLevel;

    SQLSaveManager.Group group;

    public void ActivateSession ()
    {
        ScoreSimple.sco.ResetScore();

        if(SQLSaveManager.instance)
            group = SQLSaveManager.instance.group;
        else
            print("No SaveManager found; TrialStartLogic.cs:29");
        
        trialName = gameObject.name;
        
        Control c = Control.instance;

        ScoreSimple.sco.ChangeScoreVisibility(showUI);

        c.trialDurationInMin = trialDurationInMin;

        AssistanceSelectScript.AssiSelectStates tempState = AssistanceSelectScript.AssiSelectStates.None;

        if(differentGroupAssistances)
        {
            switch (group)
            {
                case SQLSaveManager.Group.group1:
                    tempState = group1_assiLevel;
                    break;
                case SQLSaveManager.Group.group2:
                    tempState = group2_assiLevel;
                    break;
                case SQLSaveManager.Group.group3:
                    tempState = group3_assiLevel;
                    break;
                default:
                    break;
            }
        }
        else
        {
            tempState = assistanceLevel;
        }
        AssistanceSelectScript.instance.ChangeAssiSelect(tempState);

        //StartTrial
        c.StartGame(trialName, saveData, tempState.ToString());
    }
}
