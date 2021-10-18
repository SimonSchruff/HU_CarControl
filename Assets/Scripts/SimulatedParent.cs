using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatedParent : MonoBehaviour
{
    public simulationState simState = simulationState.game;

    [SerializeField] protected ContactFilter2D filterCollision;

    public List<Collider2D> colliderList = new List<Collider2D>();
    public Collider2D selfCol;
    public bool hasCollider = false;
    public enum simulationState
    {
        game,
        simulated
    }

    public virtual void UpdateSimulation (float simStep)
    {
        if(hasCollider)
        {
            int tempCount = 0;
            List<Collider2D> otherCol = new List<Collider2D>();
    //        Collider2D[] otherCol = new Collider2D[0];

            selfCol.OverlapCollider(filterCollision, otherCol);

            if(otherCol != null)
            {
                tempCount = otherCol.Count;
            }
                

            if(colliderList.Count != tempCount)
            {
                if(colliderList.Count < tempCount)
                {
                    foreach(Collider2D col in otherCol)
                    {
                        if(!colliderList.Contains(col))
                        {
                            TriggerEnterSim(col);
                            colliderList.Add(col);
                        }
                    }
                }
                if (colliderList.Count > tempCount)
                {
                    {
                        List<Collider2D> tempColListToRemove = new List<Collider2D>();
                        foreach(Collider2D selfCol in colliderList)
                        {
                            if(!otherCol.Contains(selfCol))
                            {
                                try
                                {
                                    TriggerExitSim(selfCol);
                                    tempColListToRemove.Add(selfCol);
                                } catch{}
                            }
                        }
                        foreach(Collider2D cc in tempColListToRemove)
                        {
                            colliderList.Remove(cc);
                        }
                    }
                }
            }
        }
    }
    
    public virtual void InitSimulation ()
    {
//Do Collier Test Assignment
        Collider2D tempCol;                             
        if(TryGetComponent<Collider2D> (out tempCol))
        {
            selfCol = tempCol;
            hasCollider = true;
        }

    }

    public virtual void TriggerEnterSim (Collider2D otherCol)
    {
    }
    public virtual void TriggerExitSim (Collider2D otherCol)
    {
    }
}
