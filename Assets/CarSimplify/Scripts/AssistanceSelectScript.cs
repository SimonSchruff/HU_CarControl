using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistanceSelectScript : MonoBehaviour
{
    public static AssistanceSelectScript instance;

    public AssiSelectStates assiSelectState = AssiSelectStates.None;

    [SerializeField] Text AssistanceDisplayText;

    [SerializeField] GameObject selectPanel;
    [SerializeField] Image debugNoneBut;
    [SerializeField] Image areaBut;
    [SerializeField] Image specificBut;


    public enum AssiSelectStates
    {
        Debug, None, Area, SmallArea, Specific, Select
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
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
            ChangeAssiSelect(assiSelectState);
        }
        else
        {
            print("Hide all displays!");
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
                SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.none);
                break;
            case AssiSelectStates.Area:
                AssistanceDisplayText.gameObject.SetActive(true);
                AssistanceDisplayText.text = "Area assistance ON";
                AssistanceDisplayText.color = Color.green;
                SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.areaHelp);
                break;
            case AssiSelectStates.SmallArea:
                AssistanceDisplayText.gameObject.SetActive(true);
                AssistanceDisplayText.text = "Area assistance ON";
                AssistanceDisplayText.color = Color.green;
                SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.smallAreaHelp);
                break;
            case AssiSelectStates.Specific:
                AssistanceDisplayText.gameObject.SetActive(true);
                AssistanceDisplayText.text = "Specific assistance ON";
                AssistanceDisplayText.color = Color.green;
                SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.specificHelp);
                break;
            case AssiSelectStates.Select:
                selectPanel.gameObject.SetActive(true);
                SimplAssis.instance.ChangeAssistanceMode((Random.Range(0,2)==1)?SimplAssis.AssiState.areaHelp: SimplAssis.AssiState.specificHelp);
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
    
    void SetUpDebugPanel()
    {
        if (assiSelectState == AssiSelectStates.Select && SimplAssis.instance.actualAssistance == SimplAssis.AssiState.none)
            SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.areaHelp);

        switch (SimplAssis.instance.actualAssistance)
        {
            case SimplAssis.AssiState.none:
                ChangeAssistanceToNone();
                break;
            case SimplAssis.AssiState.areaHelp:
                ChangeAssistanceToArea();
                break;
            case SimplAssis.AssiState.specificHelp:
                ChangeAssistanceToSpecific();
                break;
            case SimplAssis.AssiState.auto:
                break;
            case SimplAssis.AssiState.smallAreaHelp:
                ChangeAssistanceToSmallArea();
                break;
            default:
                break;
        }
    }

    void HideAllDispays()
    {
        AssistanceDisplayText.gameObject.SetActive(false);
        debugNoneBut.gameObject.SetActive(false);
        selectPanel.gameObject.SetActive(false);
    }


    public void ChangeAssistanceToNone ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.none);
        debugNoneBut.color = Color.green;
    }
    public void ChangeAssistanceToArea ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.areaHelp);
        areaBut.color = Color.green;

        try
        {
            Control.instance.actualSaveClass.ChangeAssitanceInGame(true);
        }
        catch {}
    }
    
    public void ChangeAssistanceToSmallArea()
    {
        ClearHighlightOfAllBut();

        SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.smallAreaHelp);
        areaBut.color = Color.green;
        
        if(Control.instance)
            Control.instance.actualSaveClass.ChangeAssitanceInGame(true);
       
    }
    
    public void ChangeAssistanceToSpecific ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.instance.ChangeAssistanceMode(SimplAssis.AssiState.specificHelp);
        specificBut.color = Color.green;

        if (Control.instance.actualSaveClass != null)
        {
            Control.instance.actualSaveClass.ChangeAssitanceInGame(false);
        }
    }

    void ClearHighlightOfAllBut ()
    {
        debugNoneBut.color = Color.white;
        specificBut.color = Color.white;
        areaBut.color = Color.white;
    }
}
