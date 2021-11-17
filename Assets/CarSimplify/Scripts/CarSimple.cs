using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimple : SimplifyParent
{
    float gridSize;
    float spawnDelay;

    Crosses[] crossLane;
    bool horOrVert;

    private void Start()
    {
        gridSize = Control.con.gridSize;
        spawnDelay = Control.con.spawnDelay;
    }

    public void ManualInit (Crosses [] CrossLane, bool HorOrVertical)
    {
        crossLane = CrossLane;
        horOrVert = HorOrVertical;
        actualStep = -1;  //Set on -1 to update on Spawn and set on 0
    }
    public void FillCrosses()
    {
        int counter = -1;

        for (int i = actualStep; i < crossLane.Length; i++)
        {
            counter++;

            if(crossLane[i] != null)
            {
                crossLane[i].crossedInTurns.Add(new Vector2(i - actualStep, horOrVert ? 0 : 1));
            }
        }

        //foreach (var cL in crossLane)
        //{
        //    counter++;

        //    if(counter >= actualStep)
        //    {
        //        if (actualStep + counter - 1 <= crossLane.Length)
        //        {
        //            if(cL != null)
        //            {
        //                cL.crossedInTurns.Add(new Vector2(actualStep - counter+1, horOrVert ? 0 : 1));
        //            }
        //        }
        //        else 
        //            break;
        //    }
        //}
    }

    private void Update()
    {
        gameObject.transform.Translate(new Vector3((gridSize * Time.deltaTime)/spawnDelay, 0, 0));
    }

}
