using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

//[ExecuteAlways]
public class CarContinueLane : SimulatedParent
{
    PathCreator pathRef;
    [SerializeField] GameObject startPosSpirte;
    

    Vector3 startPos;
    public int PathID;

    private void Awake()
    {
        pathRef = gameObject.GetComponent<PathCreator>();
        startPos = pathRef.path.GetPoint(0);
        startPosSpirte.transform.position = startPos;
    }

    void Start()
    {

    }
}
