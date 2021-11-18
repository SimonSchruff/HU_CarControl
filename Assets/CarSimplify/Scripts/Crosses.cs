using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosses : SimplifyParent
{
    public int [] TurnToLockSpwanPairs;
    public CarSpawnScript [] carSpawnLock;

    public bool actualState = false;

    public bool locked = false;


    public List<Vector2> crossedInTurns = new List<Vector2>();

    private void Start()
    {
        actualState = (Random.Range(0, 2) == 0);
        CrossClicked();
    }

    public override void SimpleUpdate()
    {
        base.SimpleUpdate();

    }

    public void ClearCrosses()
    {
        crossedInTurns.Clear();
    }

    public void CrossClicked ()
    {
        if(!locked)
        {
            actualState = !actualState;
            transform.rotation = Quaternion.Euler(0, 0, actualState ? 0 : 90);
        }
    }
}
