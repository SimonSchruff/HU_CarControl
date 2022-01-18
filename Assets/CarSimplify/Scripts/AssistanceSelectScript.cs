using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistanceSelectScript : MonoBehaviour
{
    public static AssistanceSelectScript assiSel;

    public AssiSelectStates actualAssiSelect = AssiSelectStates.None;

    [SerializeField] Text AssistanceDisplayText;

    [SerializeField] GameObject selectPanel;
    [SerializeField] Image debugNoneBut;
    [SerializeField] Image areaBut;
    [SerializeField] Image specificBut;

    [SerializeField] GameObject TutorialOverlay;
    GameObject tempOverlay;

    public enum AssiSelectStates
    {
        Debug, None, Area, Specific, Select
    }

    private void Awake()
    {
        if (assiSel == null)
        {
            assiSel = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void ChangeUIVisibility(bool changeVisibilityTo)
    {
        ScoreSimple.sco.ChangeScoreVisibility(changeVisibilityTo);
        if (changeVisibilityTo)
        {
            ChangeAssiSelect(actualAssiSelect);
        }
        else
        {
            HideAllDispays();
        }
    }

    public void ChangeAssiSelect (AssiSelectStates changeTo)
    {
        HideAllDispays();
        
        switch (changeTo)
        {
            case AssiSelectStates.Debug:
                selectPanel.gameObject.SetActive(true);
                debugNoneBut.gameObject.SetActive(true);
                break;
            case AssiSelectStates.None:
                AssistanceDisplayText.gameObject.SetActive(true);
                AssistanceDisplayText.text = "Assistance OFF";
                AssistanceDisplayText.color = Color.red;
                SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.none);
                break;
            case AssiSelectStates.Area:
                AssistanceDisplayText.gameObject.SetActive(true);
                AssistanceDisplayText.text = "Area assistance ON";
                AssistanceDisplayText.color = Color.green;
                SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.areaHelp);
                break;
            case AssiSelectStates.Specific:
                AssistanceDisplayText.gameObject.SetActive(true);
                AssistanceDisplayText.text = "Specific assistance ON";
                AssistanceDisplayText.color = Color.green;
                SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.specificHelp);
                break;
            case AssiSelectStates.Select:
                selectPanel.gameObject.SetActive(true);
                SimplAssis.assi.ChangeAssitance((Random.Range(0,2)==1)?SimplAssis.assiState.areaHelp: SimplAssis.assiState.specificHelp);
                SetUpDebugPanel();
                break;
            default:
                break;
        }
    }


    private void Start()
    {
      //  ChangeAssiSelect(actualAssiSelect);
        SetUpDebugPanel();

        ChangeUIVisibility(false);
    }
    void SetUpDebugPanel ()
    {
        if (actualAssiSelect == AssiSelectStates.Select && SimplAssis.assi.actualAssistance == SimplAssis.assiState.none)
            SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.areaHelp);

        switch (SimplAssis.assi.actualAssistance)
        {
            case SimplAssis.assiState.none:
                ChangeAssistanceToNone();
                break;
            case SimplAssis.assiState.areaHelp:
                ChangeAssistanceToArea();
                break;
            case SimplAssis.assiState.specificHelp:
                ChangeAssistanceToSpecific();
                break;
            case SimplAssis.assiState.auto:
                break;
            default:
                break;
        }
    }

    void HideAllDispays ()
    {
        AssistanceDisplayText.gameObject.SetActive(false);
        debugNoneBut.gameObject.SetActive(false);
        selectPanel.gameObject.SetActive(false);
    }


    public void ChangeAssistanceToNone ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.none);
        debugNoneBut.color = Color.green;
    }
    public void ChangeAssistanceToArea ()
    {
        CheckIfRemoveTutorialOverlay(true);

        ClearHighlightOfAllBut();

        SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.areaHelp);
        areaBut.color = Color.green;

        try
        {
            Control.con.actualSaveClass.ChangeAssitanceInGame(true);
        }
        catch {}
    }
    public void ChangeAssistanceToSpecific ()
    {
        CheckIfRemoveTutorialOverlay(false);

        ClearHighlightOfAllBut();

        SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.specificHelp);
        specificBut.color = Color.green;

        if (Control.con.actualSaveClass != null)
        {
            Control.con.actualSaveClass.ChangeAssitanceInGame(false);
        }
    }

    void CheckIfRemoveTutorialOverlay (bool toArea)
    {
        if(SimplAssis.assi.actualAssistance == (toArea ? SimplAssis.assiState.specificHelp : SimplAssis.assiState.areaHelp))
        {
            if (tempOverlay != null)
            {
                Destroy(tempOverlay);
                Time.timeScale = 1f;
            }
        }
    }

    public void CreateTutorialAssiOverlay ()
    {
        if (SQLSaveManager.instance.group == SQLSaveManager.Group.group3)
        {
            tempOverlay = Instantiate(TutorialOverlay);
            Time.timeScale = 0;
        }
    }

    void ClearHighlightOfAllBut ()
    {
        debugNoneBut.color = Color.white;
        specificBut.color = Color.white;
        areaBut.color = Color.white;
    }
}
