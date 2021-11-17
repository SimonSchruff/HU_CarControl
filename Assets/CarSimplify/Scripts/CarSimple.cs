using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimple : SimplifyParent
{
    float speed;
    float gridSize;

    Crosses[] crossLane;
    bool horOrVert;

    private void Start()
    {
        speed = Control.con.speed;
        gridSize = Control.con.gridSize;
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
        foreach (var cL in crossLane)
        {
            counter++;

            if (actualStep + counter - 1 <= crossLane.Length && actualStep - counter >= 0)
            {
                if(cL != null)
                {
                    cL.crossedInTurns.Add(new Vector2(actualStep - counter+1, horOrVert ? 0 : 1));
                }
            }
            else 
                break;
        }
    }

    private void Update()
    {
        gameObject.transform.Translate(new Vector3(gridSize * speed * Time.deltaTime, 0, 0));
    }

}
