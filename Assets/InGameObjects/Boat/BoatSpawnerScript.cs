using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

//[ExecuteAlways]
public class BoatSpawnerScript : SimulatedParent
{
    PathCreator pathRef;
    [SerializeField] GameObject startPosSpirte;
    [SerializeField] GameObject boatPrefab;
    Vector3 startPos;

    private void Awake()
    {
        pathRef = gameObject.GetComponent<PathCreator>();
        startPos = pathRef.path.GetPoint(0);
        startPosSpirte.transform.position = startPos;
    }

    public void SpawnBoatFunc ()
    {
        GameObject tempBoatRef = Instantiate(boatPrefab, startPos, Quaternion.Euler(new Vector3(0,0,0)));
        BoatControlScript bcs = tempBoatRef.GetComponent<BoatControlScript>();
        bcs.ManualStart(pathRef);

        if (simState == simulationState.simulated)
        {
            SimulationControlScript.sim.simObjects.Add(bcs);
            bcs.simState = simulationState.simulated;
        }
    }
}
