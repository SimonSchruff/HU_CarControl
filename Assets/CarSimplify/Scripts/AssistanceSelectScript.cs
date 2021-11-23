using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssistanceSelectScript : MonoBehaviour
{
    [SerializeField] Image noneBut;
    [SerializeField] Image areaBut;
    [SerializeField] Image specificBut;

    private void Start()
    {
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
    public void ChangeAssistanceToNone ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.none);
        noneBut.color = Color.green;
    }
    public void ChangeAssistanceToArea ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.areaHelp);
        areaBut.color = Color.green;
    }
    public void ChangeAssistanceToSpecific ()
    {
        ClearHighlightOfAllBut();

        SimplAssis.assi.ChangeAssitance(SimplAssis.assiState.specificHelp);
        specificBut.color = Color.green;
    }

    void ClearHighlightOfAllBut ()
    {
        noneBut.color = Color.white;
        specificBut.color = Color.white;
        areaBut.color = Color.white;
    }
}
