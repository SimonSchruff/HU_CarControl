using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplAssis : MonoBehaviour
{
    [HideInInspector] public AssiState actualAssistance = AssiState.none;

    [Header("AreaHighlight")]
    [SerializeField] SpriteRenderer[] highlightSprites;
    [SerializeField] Crosses[] crossHightlightPair_0;
    [SerializeField] Crosses[] crossHightlightPair_1;
    [SerializeField] Crosses[] crossHightlightPair_2;
    [SerializeField] Crosses[] crossHightlightPair_3;
    
    [Header("Small Area Highlight")]
    [SerializeField] SpriteRenderer[] sm_highlightSprites;
    [SerializeField] Crosses[] sm_crossHightlightPair_0;
    [SerializeField] Crosses[] sm_crossHightlightPair_1;
    [SerializeField] Crosses[] sm_crossHightlightPair_2;
    [SerializeField] Crosses[] sm_crossHightlightPair_3;
    [SerializeField] Crosses[] sm_crossHightlightPair_4;
    [SerializeField] Crosses[] sm_crossHightlightPair_5;


    float autoUpdateTimer = 0;

    public enum AssiState
    {
        none, areaHelp, specificHelp, smallAreaHelp, auto
    }

    [Serializable]
    public struct DebugData
    {
        public int totalAssiRecommendations;
        public int assiRecommendationsMissed;
        public int totalExcludedCrosses;
    }
    [Header("Debug Data")]
    public DebugData debugData;

    // Singleton
    public static SimplAssis instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        debugData = new DebugData();
    }

    public void ChangeAssistanceMode(AssiState changeStateTo)
    {
        ResetSmallHighlightAreas();
        ResetHighlightAreas();
        ResetHighlightedCrosses();
        
        actualAssistance = changeStateTo;

        UpdateAssistance();
    }

    public void UpdateAssistance()
    {
        switch (actualAssistance)
        {
            case AssiState.none:
                {
                    break;
                }
            case AssiState.areaHelp:
                {
                    ResetHighlightAreas();

                    var tempCrossToChange = SearchForNextRecommendations();
                    if(tempCrossToChange.Count > 0)
                    {
                        SpriteRenderer highlight = highlightSprites[FindHighlightAreaIndexFromCross(tempCrossToChange[0])];
                        highlight.gameObject.SetActive(true);
                                                    
                    }
                    break;
                }
            case AssiState.smallAreaHelp:
                {
                    // TODO: TEST
                    ResetSmallHighlightAreas();
                    var tempCrossToChange = SearchForNextRecommendations();
                    if(tempCrossToChange.Count > 0)
                    {
                        SpriteRenderer highlight = sm_highlightSprites[FindSmallHighlightAreaIndexFromCross(tempCrossToChange[0])];
                        highlight.gameObject.SetActive(true);
                    }
                    
                    break;
                }
                
            case AssiState.specificHelp:
                {
                    ResetHighlightedCrosses();

                    int counter = 0;
                    var tempCrossToChange = SearchForNextRecommendations();
                    foreach (var tempCross in tempCrossToChange)
                    {
                        tempCross.SetHighlighted(tempCross.tempHighlightPrio, true);
                    }

                    break;
                }
            case AssiState.auto:
                {
                    StartCoroutine(DelayAutoUpdate());
                    break;
                }
            default:
                break;
        }
    }

    int FindHighlightAreaIndexFromCross (Crosses crossToFindArea)
    {
        foreach (var area0 in crossHightlightPair_0)
        {
            if (area0 == crossToFindArea)
            {
                return 0;
            }
        }
        foreach (var area1 in crossHightlightPair_1)
        {
            if (area1 == crossToFindArea)
            {
                return 1;
            }
        }
        foreach (var area2 in crossHightlightPair_2)
        {
            if (area2 == crossToFindArea)
            {
                return 2;
            }
        }
        foreach (var area3 in crossHightlightPair_3)
        {
            if (area3 == crossToFindArea)
            {
                return 3;
            }
        }

        return -1;
    }
    
    int FindSmallHighlightAreaIndexFromCross(Crosses crossToFindArea)
    {
        foreach (var area0 in sm_crossHightlightPair_0)
        {
            if (area0 == crossToFindArea)
            {
                return 0;
            }
        }
        foreach (var area1 in sm_crossHightlightPair_1)
        {
            if (area1 == crossToFindArea)
            {
                return 1;
            }
        }
        foreach (var area2 in sm_crossHightlightPair_2)
        {
            if (area2 == crossToFindArea)
            {
                return 2;
            }
        }
        foreach (var area3 in sm_crossHightlightPair_3)
        {
            if (area3 == crossToFindArea)
            {
                return 3;
            }
        }
        foreach (var area4 in sm_crossHightlightPair_4)
        {
            if (area4 == crossToFindArea)
            {
                return 4;
            }
        }
        foreach (var area5 in sm_crossHightlightPair_5)
        {
            if (area5 == crossToFindArea)
            {
                return 5;
            }
        }

        return -1;
    }

    IEnumerator DelayAutoUpdate ()
    {
        yield return new WaitForSeconds(1f);

        //foreach (var cross in SearchForNextRecommendations())
        //{
        //    cross.CrossClicked();
        //}
    }
    public void SetUpdateTimer()
    {
        autoUpdateTimer = .5f;
    }

    private void Update()
    {
        if(autoUpdateTimer > 0)
        {
            autoUpdateTimer -= Time.deltaTime;

            if (autoUpdateTimer <= 0)
            {
                UpdateAssistance();
            }
        }
    }

    void ResetHighlightedCrosses ()
    {
        foreach (var cross in Control.instance.crossesRefs)
        {
            // Calls method with default values -> Un-highlights all crosses
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
    
    void ResetSmallHighlightAreas()
    {
        foreach (var sprite in sm_highlightSprites)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    private List<Crosses> SearchForNextRecommendations()
    {
        // ToDo: Do this in Start()
        List<Crosses> crossList = new List<Crosses>();
        Crosses[] crossRef = Control.instance.crossesRefs;

        List<Crosses> excludeCrosses = new List<Crosses>();

        // Why 6? Only 4 crosses per row
        int maxCount = 6; //Marki: How many Crosses should be checked in Row + Empty slots

        // Set highlight priority for every cross to 0
        foreach (var cr in crossRef) {
            cr.tempHighlightPrio = 0;
        }
        
        for (int i = 0; i < maxCount; i++)
        {
            foreach (var cross in crossRef)
            {
                foreach (var vec in cross.crossedInTurnsDictionary)
                {
                    bool possible = true;

                    // Check if action might be needed in close turn
                    if(vec.Value.x == i) 
                    {
                        if(cross.actualState != (vec.Value .y == 0))
                        {
                            if (i != 0)
                            {
                                foreach (var usedCrosses in crossList)
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
                                
                                if (possible )
                                {
                                    
                                    if(cross.tempHighlightPrio == 0 && !vec.Key)
                                    {
                                        //print("Highlighted Cross: " + cross.gameObject.name);
                                        cross.tempHighlightPrio = i + 1;
                                        debugData.totalAssiRecommendations++;
                                        crossList.Add(cross);
                                    }

                                    if (vec.Key)
                                    {
                                        debugData.assiRecommendationsMissed++;
                                    }
                                }
                            }
                        }
                        else    
                        {
                            //Marki: Exclude when cross needs to be crossed and cross standing right
                            // Exclude cross when already in the correct position
                            //print("Excluded Cross: " + cross.gameObject.name);
                            debugData.totalExcludedCrosses++;
                            excludeCrosses.Add(cross);
                        }
                    }
                }
            }
        }
        
        
        
        
        

        /*
        for (int i = 0; i < maxCount; i++)
        {
            foreach (var cross in crossRef)
            {
                foreach (Vector2 vec in cross.crossedInTurns)
                {
                    bool possible = true;

                    // Check if action might be needed in close turn
                    if(vec.x == i) 
                    {
                        if(cross.actualState != (vec.y == 0))
                        {
                            if (i != 0)
                            {
                                foreach (var usedCrosses in crossList)
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
                                    if(cross.tempHighlightPrio == 0)
                                    {
                                        //print("Highlighted Cross: " + cross.gameObject.name);
                                        cross.tempHighlightPrio = i + 1;
                                        crossList.Add(cross);
                                    }
                                }
                            }
                        }
                        else    
                        {
                            //Marki: Exclude when cross needs to be crossed and cross standing right
                            // Exclude cross when already in the correct position
                            //print("Excluded Cross: " + cross.gameObject.name);
                            excludeCrosses.Add(cross);
                        }
                    }
                }
            }
        }
        */

        return crossList;
    }


}
