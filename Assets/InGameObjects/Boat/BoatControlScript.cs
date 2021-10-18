using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class BoatControlScript : SimulatedParent
{
    PathCreator pathRef;
    public float timeCounter;
    public float boatSpeed = .2f;
    public float pathLength;

    public void ManualStart (PathCreator pathToFollow)
    {
        pathRef = pathToFollow;
        pathLength = pathRef.path.length;
    }

    public override void UpdateSimulation(float simStep)
    {
        base.UpdateSimulation(simStep);

        ManualUpdate(simStep);
    }

    private void Update()
    {
        if(simState == simulationState.game)
            ManualUpdate(Time.deltaTime);
    }

    void ManualUpdate(float step)
    {
        if (pathRef == null)
            pathRef = SimulationControlScript.sim.boatSpawnerSim.gameObject.GetComponent<PathCreator>();

        timeCounter += step;
        transform.position = pathRef.path.GetPointAtDistance(boatSpeed *timeCounter);
        Quaternion rot = pathRef.path.GetRotationAtDistance(boatSpeed * timeCounter);
        rot *= Quaternion.Euler(Vector3.up *90);
        transform.rotation = rot;

        if (pathLength < boatSpeed * timeCounter)        //CheckIfFinieshed
        {
            BoatAtEndOfTrack();
        }
    }

    void BoatAtEndOfTrack ()     //Car finieshed line - At end of track
    {
        Destroy(gameObject);
    }
}
