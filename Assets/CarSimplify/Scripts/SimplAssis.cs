using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplAssis : MonoBehaviour
{
    public assiState actualAssistance = assiState.none;

    [Header("AreaHighlight")]
    [SerializeField] SpriteRenderer[] highlightSprites;
    [SerializeField] Crosses[] crossHightlightPair_0;
    [SerializeField] Crosses[] crossHightlightPair_1;
    [SerializeField] Crosses[] crossHightlightPair_2;
    [SerializeField] Crosses[] crossHightlightPair_3;

    float autoUpdateTimer = 0;

    public static SimplAssis assi;
    public enum assiState
    {
        none,
        areaHelp,
        specificHelp,
        auto
    }

    private void Awake()
    {
        if (assi == null)
            assi = this;
        else
            Destroy(this);
    }

    public void ChangeAssitance (assiState changeStateTo)
    {
        ResetHighlightAreas();

        ResetHighlightedCrosses();


        actualAssistance = changeStateTo;

        UpdateAssistance();
    }

    public void UpdateAssistance ()
    {
        switch (actualAssistance)
        {
            case assiState.none:
                {

                    break;
                }
            case assiState.areaHelp:
                break;
            case assiState.specificHelp:
                {
                    ResetHighlightedCrosses();

                    int counter = 0;
                    foreach (var cross in SearchForNextRecommendations())
                    {
                        counter++;
                        cross.SetHighlighted(counter, true);
                    }

                    break;
                }
            case assiState.auto:
                {
                    StartCoroutine(DelayAutoUpdate());

                    break;
                }
            default:
                break;
        }
    }
    IEnumerator DelayAutoUpdate ()
    {
        yield return new WaitForSeconds(1f);

        foreach (var cross in SearchForNextRecommendations())
        {
            cross.CrossClicked();
        }
    }
    public void SetUpdateTimer()
    {
        autoUpdateTimer = .5f;
    }

    private void Update()
    {
        if(autoUpdateTimer>0)
        {
            autoUpdateTimer -= Time.deltaTime;

            if (autoUpdateTimer<= 0)
            {
                UpdateAssistance();
            }
        }
    }

    void ResetHighlightedCrosses ()
    {
        foreach (var cross in Control.con.crossesRefs)
        {
            cross.SetHighlighted();
        }
    }
    void ResetHighlightAreas()
    {
        foreach (var sprite in highlightSprites)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    List <Crosses>  SearchForNextRecommendations ()
    {
        List<Crosses> crosList = new List<Crosses>();
        Crosses[] crosRef = Control.con.crossesRefs;

        List<Crosses> excludeCrosses = new List<Crosses>();


        for (int i = 0; i < 6; i++)
        {
            foreach (var cross in crosRef)
            {
                foreach (Vector2 vec in cross.crossedInTurns)
                {
                    if(vec.x == i) // Check if action might be needed in close turn
                    {
                        if(cross.actualState != (vec.y==0))
                        {
                            bool possible = true;
                            foreach (var usedCrosses in crosList)
                            {
                                foreach(var prevCross in (cross.actualState? cross.previousCrossH: cross.previousCrossV))
                                {
                                    if(prevCross == usedCrosses)
                                    {
                                        possible = false;
                                        break;
                                    }
                                }
                                if (possible == false)
                                {
                                    break;
                                }
                            }
                            if (possible)
                            {
                                foreach (var exlCross in excludeCrosses)
                                {
                                    if (cross == exlCross)
                                    {
                                        possible = false;
                                        break;
                                    }
                                }
                                if (possible)
                                {
                                    crosList.Add(cross);
                                }
                            }
                        }
                        else    //Exclude when cross needs to be crossed and cross standing right
                        {
                            excludeCrosses.Add(cross);
                        }
                    }
                }
            }
        }




        return crosList;
    }


}
