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

    public static SimplAssis assi;
    public enum assiState
    {
        none,
        areaHelp,
        specificHelp
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
        foreach (var sprite in highlightSprites)
        {
            sprite.gameObject.SetActive(false);
        }
        foreach (var cross in Control.con.crossesRefs)
        {
            cross.SetHighlighted(false);
        }


        actualAssistance = changeStateTo;
    }


    private void Update()
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
                break;
            default:
                break;
        }
    }

    List <Crosses>  SearchForNextRecommendations ()
    {
        List<Crosses> crosList = new List<Crosses>();
        Crosses[] crosRef = Control.con.crossesRefs;

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
                            bool possible = false;
                            foreach (var usedCrosses in crosList)
                            {
                              //  if((cross.actualState? cross.previousCrossH: cross.previousCrossV).
                            }
                        }

                    }
                }
            }
        }




        return crosList;
    }


}
