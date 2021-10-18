using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

//[ExecuteAlways]
public class CarSpawner : SimulatedParent
{
    PathCreator pathRef;
    [SerializeField] GameObject startPosSpirte;
    [SerializeField] GameObject carPrefabRef;
    [SerializeField] GameObject emergencyCarPrefabRef;
    Vector3 startPos;
    public int PathID;

    private void Awake()
    {
        pathRef = gameObject.GetComponent<PathCreator>();
        startPos = pathRef.path.GetPoint(0);
        startPosSpirte.transform.position = startPos;
    }

    public void SpawnCar (bool isEmergency = false)
    {
        GameObject tempCarRef = Instantiate(isEmergency ? emergencyCarPrefabRef:carPrefabRef, startPos, Quaternion.Euler(new Vector3(0,0,0)));
        CarControlScript ccs = tempCarRef.GetComponent<CarControlScript>();
        ccs.ManualStart(pathRef, PathID);

        if (simState == simulationState.simulated)
        {
            try
            {
                ccs.simState = simulationState.simulated;
                SimulationControlScript.sim.simObjects.Add(ccs);
                CarInFrontDetect carInFront = ccs.GetComponentInChildren<CarInFrontDetect>();
                carInFront.simState = simulationState.simulated;
                SimulationControlScript.sim.simObjects.Add(carInFront);
                SimulationControlScript.sim.simCars.Add(ccs);
                ccs.InitSimulation();
                carInFront.InitSimulation();
            }
            catch { }

        }
    }
}
