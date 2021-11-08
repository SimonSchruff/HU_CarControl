using System.Collections;
using System.Collections.Generic;
using PathCreation;
using UnityEngine;

public class CarControlScript : SimulatedParent
{
    [SerializeField] Score.PointTypes finishPointTyp = Score.PointTypes.carFinished;
    [SerializeField] Score.PointTypes crashPointTyp = Score.PointTypes.carCrash;
    [SerializeField] Score.PointTypes inWaterPointTyp = Score.PointTypes.carInWater;

    public bool isEmergency = false;
    PathCreator pathRef;
    public int pathCounter;
    public float timeCounter;
    public float carSpeed = .5f;
    float pathLength;
    public List <int> pathID = new List<int>();
    public driveState state = driveState.gameNotStated;

    // Emergency 
    public float emergencyReduceScoreDelay = 1f;
    public float emergencyCounter = 0f;

    [SerializeField] ContactFilter2D contactFilterContLane;

    crashState selfCrashState = crashState.healthy;

    //TrafficLights
    public List<int> crossedTrafficLightIDs = new List<int>();
    public int actualWaitingLightID;
    public int carsInRowCounter;
    TrafficLightScript waitForGreenTrafLightRef;

    //CarInFront
    public double waitCarInFrontStartTime = 0;
    public float minBridgeCrossTime = .5f;

    //SIM
    float simSteps;
    bool isDestroy = false;
    float destroyTimer = .5f;

    bool gotPointsForFinish = false;


    //ENUMS
    public enum driveState
    {
        gameNotStated,
        driving,
        waitingAtTrafficLight,
        waitingCarInFront
    }
    public enum crashState
    {
        healthy,
        crashCar,
        inWater
    }

    public void ManualStart (PathCreator pathToFollow, int pathIDFromSpawner, bool isInit = true)
    {
        pathRef = pathToFollow;
        if(isInit)
            pathID.Add(pathIDFromSpawner);
        state = driveState.driving;
        pathLength = pathRef.path.length;
        timeCounter = 0;
    }

    public override void InitSimulation()       //What to do when Simulation starts
    {
        base.InitSimulation();

        PathCreator tempPath;
    //    state = driveState.driving;

        foreach (SimulatedParent sp in SimulationControlScript.sim.simObjects)
        {
            if (sp.TryGetComponent<PathCreator>(out tempPath))           //Check if look Up object is CarSpawner
            {
                CarSpawner cs;
                CarContinueLane ccl;
                if (tempPath.gameObject.TryGetComponent<CarSpawner>(out cs))
                {
                    try
                    {
                        if (cs.PathID == pathID[pathCounter])
                        {
                            pathRef = tempPath;
                            pathLength = pathRef.path.length;

                            break;
                        }
                    }
                    catch { }
                }

                else if (tempPath.gameObject.TryGetComponent<CarContinueLane>(out ccl))        // check if look up object is continuePath obj
                {
                    try
                    {
                        if (ccl.PathID == pathID[pathCounter])
                        {
                            pathRef = tempPath;
                            pathLength = pathRef.path.length;

                            break;
                        }
                    }
                    catch { }
                    
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(simState == simulationState.game)
            UpdateManual();
    }

    public override void UpdateSimulation(float simStep)
    {
        base.UpdateSimulation(simStep);     //Call Parent Trigger Control
        simSteps = simStep;
        UpdateManual();
    }

    void UpdateManual()
    {
        if (selfCrashState == crashState.healthy)
        {
            if (state != driveState.gameNotStated)
            {
                if (state == driveState.driving)
                {
                    try
                    {
                        timeCounter += simState == simulationState.game ? Time.deltaTime : simSteps;
                        transform.position = pathRef.path.GetPointAtDistance(carSpeed * timeCounter);
                        //    transform.rotation = pathRef.path.GetRotationAtDistance(carSpeed * timeCounter);
                        Quaternion rot = pathRef.path.GetRotationAtDistance(carSpeed * timeCounter);
                        rot *= Quaternion.Euler(Vector3.up * 90);
                        transform.rotation = rot;

                        //    float rot = pathRef.path.GetRotationAtDistance(carSpeed * timeCounter).eulerAngles.x;
                        //    transform.rotation = Quaternion.Euler(0, 0, rot);

                        if(simState == simulationState.simulated)
                        {
                        //      Physics2D.SyncTransforms();
                         //   transform.Translate(Vector3.zero);
        //                    GetComponent<SpriteRenderer>().color = Random.ColorHSV();
                        }
                    }
                    catch { }

                        if (pathLength < carSpeed * timeCounter)        //CheckIfFinieshed
                        {
                            CarAtEndOfTrack();
                        }
                }

                if (state == driveState.waitingAtTrafficLight)
                {
                    try         //Check if traffic light is green, when no ref to the light - cause sim then searches for the simulated
                    {
                        if(actualWaitingLightID == 0)       //Bug fix - no traffic light ID assigned
                        {
                            actualWaitingLightID = waitForGreenTrafLightRef.trafficLightID;
                        }

                        if (waitForGreenTrafLightRef.state == TrafficLightScript.lightState.green)
                        {
                            carsInRowCounter = 0;
                            actualWaitingLightID = 0;
                            state = driveState.driving;
                        }
                    }
                    catch
                    { 
                        if(simState == simulationState.simulated)
                        {
                            foreach (TrafficLightScript tl in SimulationControlScript.sim.simTrafficLights)
                            {
                                if(actualWaitingLightID == tl.trafficLightID)
                                {
                                    waitForGreenTrafLightRef = tl;
                                    break;
                                }
                            }
                        }
                    }
                }

                if(state == driveState.waitingAtTrafficLight || state == driveState.waitingCarInFront)
                {
                    emergencyCounter += simState == simulationState.game ? Time.deltaTime : simSteps;

                    if (isEmergency)
                    {
                        if(emergencyCounter >= emergencyReduceScoreDelay)
                        {
                            Score.sc.AddPoints(Score.PointTypes.emergencyWait, simState);
                            emergencyCounter = 0f;
                            if (simState == simulationState.simulated)  // Add Priority to traffic light for waitng
                            {
                                if (actualWaitingLightID != 0)
                                {
                                    SimulationControlScript.sim.AddScoreToTrafficLight(actualWaitingLightID, 10);

                                    SimulationControlScript.sim.GetTrafficLightRefFromID(actualWaitingLightID).AddEntryToDebugListing("EmergencyWait",10,gameObject);
                                }
                                else
                                    Debug.Log("Emergency Error Here");
                            }
                        }
                    }
                    else        // When no emergency Car - Add priority to traffic light as well
                    {
                        if(emergencyCounter >= emergencyReduceScoreDelay)
                        {
                            emergencyCounter = 0f;
                            if (simState == simulationState.simulated)  // Add Priority to traffic light for waitng
                            {
                                if (actualWaitingLightID != 0)
                                {
                                    //   CheckIfLightIsAlreadyGreen
                                    SimulationControlScript tempSim = SimulationControlScript.sim;
                                    bool isChangingTrafficLight = false;
                                    if(tempSim.trafficLightsToTest.Count > 0)      //Check if wait in Traffic light cause of change of phases
                                    {
                                        isChangingTrafficLight = tempSim.trafficLightsToTest[0] == actualWaitingLightID;
                                    }
                                    int valueToAdd = isChangingTrafficLight ? -1 : 1;

                                    if (tempSim.GetTrafficLightRefFromID(actualWaitingLightID).state != TrafficLightScript.lightState.green)
                                    {
                                        tempSim.AddScoreToTrafficLight(actualWaitingLightID, valueToAdd);

                                        tempSim.GetTrafficLightRefFromID(actualWaitingLightID).AddEntryToDebugListing("NormalCarWaiting", valueToAdd, gameObject);
                                    }
                                }
                               // else
                                 //   Debug.Log("Normal Car Error Here");
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (isDestroy)
            {
                destroyTimer -= simSteps;
                if(destroyTimer <= 0)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
    }

    void CarAtEndOfTrack ()     //Car finieshed line - At end of track
    {
      //  CheckIfOtherCarWouldStop();


        Collider2D col = GetComponent<Collider2D>();
        List<Collider2D> tempColliders = new List<Collider2D>();
        if (col.OverlapCollider(contactFilterContLane, tempColliders) > 0)
        {
            pathCounter++;
            GameObject PathCont = tempColliders[0].transform.parent.gameObject;

            ManualStart(PathCont.GetComponent<PathCreator>(), PathCont.GetComponent<CarContinueLane>().PathID, false);
            UpdateManual();
        }
        else
        {
            Score.sc.AddPoints(finishPointTyp , simState);
            DestroyImmediate(gameObject);
        }
    }

    void CheckIfOtherCarWouldStop ()
    {
        if(simState == simulationState.game)
        {
            List <Collider2D> collidingObjects = new List<Collider2D>();
            GetComponent<Collider2D>().OverlapCollider(filterCollision, collidingObjects);
            foreach (Collider2D col in collidingObjects)
            {
                if(col.CompareTag("CarOuter"))
                {
                    try
                    {
                        if (col.GetComponent<CarInFrontDetect>().selfCar.state == driveState.waitingCarInFront)
                        {
                            Debug.Log("Destroy when car is behind");
                        }
                        break;
                    }
                    catch { } // Catch when no self car ref is there any more
                }
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)     // When normal game
    {
        if(simState == simulationState.game)
        {
            ManualTriggerCheck(collision);
        }
    }

    public override void TriggerEnterSim(Collider2D otherCol)
    {
        if(simState == simulationState.simulated)
        {
            ManualTriggerCheck(otherCol);
        }
    }

    private void ManualTriggerCheck(Collider2D collision)       // Trigger check needs to work also in sim
    {
       if (state == driveState.driving)
       {
            if (collision.gameObject.CompareTag("TrafficLight"))
            {
                if (simState == simulationState.simulated)      // Debug purposes
                {
                }
                TrafficLightScript tls = collision.GetComponent<TrafficLightScript>();
                if (tls.state != TrafficLightScript.lightState.green)                   //CheckIfTrafficLightIsNotGreen
                {
                    StopCarAtTrafficLight(tls);     // Call Stop Car
                    carsInRowCounter = 1;
                }
                //Save Traffic Light To list to not stop there again
                crossedTrafficLightIDs.Add(tls.trafficLightID);
            }
            else if (collision.gameObject.CompareTag("CarInner"))
            {
                if(collision.GetComponent<SimulatedParent>().simState == simState)
                {
                    CrashHappened(crashState.crashCar, collision);
                }
            }
            else if (collision.gameObject.CompareTag("ChangeLane"))
            {
                GameObject PathCont = collision.transform.parent.gameObject;
                pathID.Add(PathCont.GetComponent<CarContinueLane>().PathID);
            }
       }
    }

    void StopCarAtTrafficLight(TrafficLightScript trafficLightScript)
    {
        bool wasCrossed = false;
        foreach (int tempTLid in crossedTrafficLightIDs)        //Check if Traffic light was already crossed
        {
            if (trafficLightScript.trafficLightID == tempTLid)
            {
                wasCrossed = true;
                return;
            }
        }
        if (!wasCrossed)
        {
            state = driveState.waitingAtTrafficLight;
            waitForGreenTrafLightRef = trafficLightScript;
            actualWaitingLightID = trafficLightScript.trafficLightID;

            StopEmergency();

            if (simState == simulationState.simulated)
            {
                if (actualWaitingLightID != 0)
                {
      //              SimulationControlScript.sim.AddScoreToTrafficLight(actualWaitingLightID, 1);

     //               SimulationControlScript.sim.GetTrafficLightRefFromID(actualWaitingLightID).AddEntryToDebugListing("FirstCarWaitAtTrafficLight",1,gameObject);
                }
            }
        }
    }


    public void StopCarInFront (bool stop)
    {
        if(stop)
        {
            state = driveState.waitingCarInFront;
            waitCarInFrontStartTime = Time.realtimeSinceStartupAsDouble;
            StopEmergency();
        }
        else
        {
            state = driveState.driving;
            actualWaitingLightID = 0;
        }
    }

    void StopEmergency()
    {
        if(isEmergency)
        {
            emergencyCounter = 0f;
        }
    }
    IEnumerator ReduceScoreEmergency ()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (state == driveState.waitingCarInFront || state == driveState.waitingAtTrafficLight)
        {
            Score.sc.AddPoints(Score.PointTypes.emergencyWait, simState);
            StartCoroutine(ReduceScoreEmergency());
        
        }
    }
    
    public void CrashHappened (crashState crashReason, Collider2D otherCol = null)
    {
        if (crashReason == crashState.crashCar && otherCol != null)     //Make sure other car gets destroyed as well
            otherCol.GetComponent<CarControlScript>().CrashHappened(crashState.crashCar);
        if(selfCrashState == crashState.healthy)
        {
            if (crashReason == crashState.inWater)      //When fell in water
            {
                Score.sc.AddPoints(inWaterPointTyp, simState);
                SendInfoToSim(inWaterPointTyp);
            }
            else                //When car crash
            {
                Score.sc.AddPoints(crashPointTyp, simState);
                SendInfoToSim(crashPointTyp);
            }
            selfCrashState = crashReason;
        }

        if(simState == simulationState.game)
            StartCoroutine(DestroyAfterFeedback());
        else
            isDestroy = true;
    }

    void SendInfoToSim (Score.PointTypes pointTyp)
    {
        if(simState == simulationState.simulated)
        {
            Score s = Score.sc;
            int amount = 0;
            switch (pointTyp)        // Assign Point values to types
            {
                case Score.PointTypes.carCrash:
                    amount = s.carCrash;
                    break;
                case Score.PointTypes.emergencyCrash:
                    amount = s.emergencyCrash;
                    break;
                case Score.PointTypes.carInWater:
                    amount = s.carInWater;
                    break;
                case Score.PointTypes.emergencyInWater:
                    amount = s.emergencyInWater;
                    break;
//                case Score.PointTypes.emergencyWait:
//                    amount = -emergencyWait;
//                    break;
            }

//WhenCarWaitsInRow
            if (actualWaitingLightID != 0 && state == driveState.waitingCarInFront) // Check if crash cause of 
            {
               SimulationControlScript.sim.AddScoreToTrafficLight(actualWaitingLightID, s.carCrash * 100);
                //DEBUG 1
                SimulationControlScript.sim.GetTrafficLightRefFromID(actualWaitingLightID).AddEntryToDebugListing("InRowWaitingCrash",amount,gameObject);
                return;
            }

            List<Collider2D> otherCol = new List<Collider2D>();
            selfCol.OverlapCollider(filterCollision, otherCol);

            //     SpotSimScript spot = null;
            //    bool found = false;
            //    int tempToAdd = amount * 100;
            //    int tempWaiting = 0;

            //try
            //{
            //    tempWaiting = SimulationControlScript.sim.GetTrafficLightRefFromID(crossedTrafficLightIDs[crossedTrafficLightIDs.Count - 1]).waitingCarsCounter;
            //}
            //catch { Debug.Log("FAILED_0"); }

            //try
            //{
            //    if (tempWaiting > 1)
            //    {
            //        tempToAdd = tempToAdd - (Mathf.Clamp(tempWaiting, 1, 9) * 9);
            //        Debug.Log(tempToAdd);
            //    }
            //}
            //catch { Debug.Log("FAILED_1"); }
            try
            {

                SimulationControlScript.sim.AddScoreToTrafficLight(crossedTrafficLightIDs[crossedTrafficLightIDs.Count - 1], amount * 100);
            }
            catch
            {
                Debug.Log("FAILED_11");
                Destroy(gameObject);
            }
            try
            {
                SimulationControlScript.sim.GetTrafficLightRefFromID(crossedTrafficLightIDs[crossedTrafficLightIDs.Count - 1]).AddEntryToDebugListing("CarSpawnCollision", amount*100,gameObject);
            }
            catch 
            { 
                Debug.Log("FAILED_22"); 
            }
            /*
            foreach(Collider2D col in otherCol)
            {
                if (col.gameObject.TryGetComponent<SpotSimScript>(out spot))
                {
                    found = true;
                    break;
                }
            }
            


            if (found && spot != null)
            {
       //         SimulationControlScript.sim.AddCrash(spot, amount, this);

//DEBUG 4
                foreach(TrafficLightScript tl in spot.trafficLights)
                {
     //               SimulationControlScript.sim.GetTrafficLightRefFromID(tl.trafficLightID).AddEntryToDebugListing("SimpleSpotCollide",amount,gameObject);
                }
            }
            else
            {

            //CheckIfKilledCauseOfJamTillSpawnPoint
                Collider2D colSpawnCar = null;
                bool found2 = false;
                foreach(Collider2D col in otherCol)
                {
                    if (col.CompareTag("CarSpawnCol"))
                    {
                        colSpawnCar = col.GetComponent<Collider2D>();
                        found2 = true;
                        break;
                    }
                }
                if(found2)  //Find other car in Row thats stock at traffic light and add priority
                {
                    List<Collider2D> otherSpawnCol = new List<Collider2D>();
                    colSpawnCar.OverlapCollider(filterCollision, otherSpawnCol);
                    foreach (Collider2D coll in otherSpawnCol)
                    {
                        CarControlScript ccs;
                        if(coll.gameObject.TryGetComponent<CarControlScript>(out ccs))
                        {
                            if (ccs.actualWaitingLightID != 0 && ccs.state == driveState.waitingCarInFront)
                            {
             //                   SimulationControlScript.sim.AddScoreToTrafficLight(ccs.actualWaitingLightID, s.carCrash*10);
                                //DEBUG 1
             //                   SimulationControlScript.sim.GetTrafficLightRefFromID(ccs.actualWaitingLightID).AddEntryToDebugListing("CarSpawnCollision",amount,ccs.gameObject);
                                break;

                            }
                        }
                    }
                }
            }
                */
        }
    }

    IEnumerator DestroyAfterFeedback ()
    {
        GetComponent<Animation>().Play();
        yield return new WaitForSeconds(.5f);       //Kill Car after time
        Destroy(gameObject);
    }

}
