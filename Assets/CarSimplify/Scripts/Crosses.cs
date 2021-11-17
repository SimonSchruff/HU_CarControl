using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosses : SimplifyParent
{
    public int [] TurnToLockSpwanPairs;
    public CarSpawnScript [] carSpawnLock;


    public List<Vector2> crossedInTurns = new List<Vector2>();

    public override void SimpleUpdate()
    {
        base.SimpleUpdate();

    }

    public void ClearCrosses()
    {
        crossedInTurns.Clear();
    }
}
