using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimulationControlScript : MonoBehaviour
{
    public bool debug = true;

    public static SimulationControlScript sim;

    [SerializeField] float offsetFromOriginal = 20;
    [SerializeField] float simulationSteps = .1f;
    [SerializeField] int simulationStepsAmount = 100;
    [SerializeField] float simulationStepRepeat = 20;
    [SerializeField] float timeScale = 1;
    [SerializeField] bool isDelaySim = true;
    [SerializeField] bool destroyRenderers = false;
    [SerializeField] int simulationDepth = 2;
    [SerializeField] float reactionTime = .5f;
    [Header("simStepAmount * simSteps * simStepRepeat")]
    [Header("time in future is calcualted through:")]
    public List<SimulatedParent> simObjects = new List<SimulatedParent>();
    public List<CarControlScript> simCars = new List<CarControlScript>();
    public List<TrafficLightScript> simTrafficLights = new List<TrafficLightScript>();
    public Dictionary<int, TrafficLightScript> simTrafficLightsDict = new Dictionary<int, TrafficLightScript>();

    [SerializeField] List<SimEvent> simEvents = new List<SimEvent>();

    //CopyFromGameManager
    public float simTimeCounter;
    [SerializeField] List<CarSpawner> carSpawnerList = new List<CarSpawner>();
    public BoatSpawnerScript boatSpawnerSim;
    [SerializeField] List<int> orderOfSpawnCarLocSim = new List<int>();

    [SerializeField] List<float> carSpawnTimesSim = new List<float>();
    List<float> boatSpawnTimesSim = new List<float>();
    List<float> emergencySpawnTimesSim = new List<float>();

    [SerializeField] bool nextCarEmergency = false;
    [SerializeField] int actualSimRepeats = 0;

    List<GameObject> debugTrafficLightsScores = new List<GameObject>();
    [SerializeField] GameObject debugTrafficLightPrefab;

    Score scoreRef;
    public int simScore;
    float simCounter;
    float trafficLightCounter;

    bool simFinished = true;
    public bool inNotTryChangeTrafficLight = false;

    public Dictionary<int, int> IdPriorityMap = new Dictionary<int, int>();

    public Dictionary<int, int> trafficLightScores = new Dictionary<int, int>();

    // Automatic Sim control
    public List<int> trafficLightsToTest = new List<int>();
    [SerializeField] int actualDepth = 0;
    int checkLightsCounter = 0;
    [SerializeField] List<int> changeTrafficLights = new List<int>();
    [SerializeField] Dictionary<int, float> durationMap = new Dictionary<int, float>();

    public List<TrafficLightScript> lightsToChangeInThisRun = new List<TrafficLightScript>();

    public List<int> lightsToIgnore = new List<int>();

   // public List<int> trafficLightsToChangeIDs = new List<int>();

    //Traffic light prioritize
    int oldTLHighestScore = 0;
    int simFrameCounter = 0;

    List<int> checkedTL = new List<int>();
    Dictionary<int, int> copyOfStartScores = new Dictionary<int, int>();

    //ToDeleteList
    public List<SimulatedParent> toDeleteList = new List<SimulatedParent>();


    [SerializeField] int successfullyTestedTL = 0;
    [SerializeField] int correspondingTLScore = 0;
    [SerializeField] int correspondingTLID = 0;
    [SerializeField] int addedScoreFirstCheck = 0;
    [SerializeField] int actualTestCounter = 0;

    [SerializeField] SimEvent.checkState actualSimState;
    [SerializeField] GameObject SimEventPrefab;


    private void Awake()
    {
        if (sim == null)
        {
            sim = this;
        }
        else
            Destroy(this.gameObject);

        if(SimEventPrefab == null)
            Debug.LogError("Assign simEventPrefab!");
    }

    private void Start()
    {
        StartCoroutine(StartSimCoroutine());
    }
    IEnumerator StartSimCoroutine()
    {
        yield return new WaitForSecondsRealtime(1f);
        if (GameManager.GM.canGameStart)
        {
            if (Assistance.assi.actualAssistance != Assistance.AssistanceTypes.Manual)
                StartSim();
            else
                StartCoroutine(StartSimCoroutine());
        }
        else
            StartCoroutine(StartSimCoroutine());
    }

    void FinishPart ()
    {
        simFrameCounter++;

        bool doCheckAtEndOfMethod = true; //Change this in function to not continue

        int testedMaxTL = 0;    //Set in beginning of method
        int TLPairID = 0;
        int testedScore = 0;
        int pairScore = 0;
        string debug = "";
        Dictionary<int, int> TLScores =GetTLScoresAsDictionary();

        bool tempCheckFailed = false;

        if (simEvents.Count > 0)
            tempCheckFailed = simEvents[simEvents.Count - 1].selfCheckState == SimEvent.checkState.fistCheckFail;


        if (simEvents.Count == 0 || tempCheckFailed) 
        {
            actualSimState = SimEvent.checkState.init;      //Init State

            testedMaxTL = GetTLIDFromMaxScore();
            if(testedMaxTL != 0)
            {
                trafficLightsToTest.Add(testedMaxTL);
                debug = "Added tl to test";
            }
            else
            {
                debug = "No tl has score";
            }
        }

        else if(simEvents[simEvents.Count-1].selfCheckState != SimEvent.checkState.firstCheck)
        {
            actualSimState = SimEvent.checkState.firstCheck;     //FirstCheck

            SimEvent tempLastSimE = simEvents[simEvents.Count - 1];
            if(trafficLightsToTest.Count > 0)
            {
                testedMaxTL = trafficLightsToTest[0];
                trafficLightsToTest.Clear();

                debug = "TL was tested: " + testedMaxTL;

                if (TLScores[testedMaxTL] < tempLastSimE.testedScore)  // when old score was higher
                {
                    if (!lightsToIgnore.Contains(tempLastSimE.pairTrafficLight))        //Check if pair TL is already in the Ignore list
                        trafficLightsToTest.Add(tempLastSimE.pairTrafficLight);
                    else
                    {
                        FinalizeSimulation(testedMaxTL);
                        doCheckAtEndOfMethod = false;
                        Debug.Log("PairTL is already in the ignore list!");
                    }
                }
                else        //When old score was better than simulated
                {
                    lightsToIgnore.Add(testedMaxTL);
                    actualSimState = SimEvent.checkState.fistCheckFail;
                    doCheckAtEndOfMethod = false;
                    StartCoroutine(ContinueSimAfterFrame());
                }
            }
            else
            {
                debug = "no traffic light was tested";
                Debug.Log("Why no tl in first check?!");
            }
        }
        else 
        {
            actualSimState = SimEvent.checkState.pairCheck;    //PairCheck

            SimEvent tempLastSimE = simEvents[simEvents.Count - 1];
            testedMaxTL = trafficLightsToTest[0];
            trafficLightsToTest.Clear();

            Debug.Log("Last: aS_" + tempLastSimE.testedScore + " pS_" + tempLastSimE.pairTLsScore);
            Debug.Log("this: aS_" + TLScores [testedMaxTL] + " pS_" + TLScores[GetSimTLFromID(testedMaxTL).correspondingTLID]);

            int sumOld = tempLastSimE.testedScore + tempLastSimE.pairTLsScore;
            int sumNew = TLScores[testedMaxTL] + TLScores[GetSimTLFromID(testedMaxTL).correspondingTLID];
            if (sumOld > sumNew)
            {
                FinalizeSimulation(testedMaxTL);
                doCheckAtEndOfMethod = false;
                Debug.Log("corresponding tl was took");
            }
            else
            {
                FinalizeSimulation(GetSimTLFromID(testedMaxTL).correspondingTLID);
                doCheckAtEndOfMethod = false;
                Debug.Log("took previous tl");
            }
        }

        SimEvent tempSimEvent = Instantiate(SimEventPrefab).GetComponent<SimEvent>();
        simEvents.Add(tempSimEvent);       //add to SimEvents

        if (testedMaxTL != 0)    //Set fixed Variables
        {
            testedScore = TLScores[testedMaxTL];
            TLPairID = GetSimTLFromID(testedMaxTL).correspondingTLID;
            pairScore = TLScores[TLPairID];
        }

        // Assign variables to SimEvent
        tempSimEvent.SetAllParameters(actualSimState, simFrameCounter, testedMaxTL, TLPairID, testedScore, pairScore, TLScores, debug);

        ClearSimCache();

        if(doCheckAtEndOfMethod)
        {
            if (trafficLightsToTest.Count > 0)      //Check if continue another
            {
                StartCoroutine(ContinueSimAfterFrame());
            }
            else
            {
                FinishSim();
            }
        }
    }
    void FinalizeSimulation (int recommendation)
    {
        RecommendTrafficLight(recommendation, simFrameCounter);
        trafficLightsToTest.Clear();
        FinishSim();
    }


    void FinishPartOld()
    {
        bool isFirstTLTest = trafficLightsToTest.Count == 0;
        inNotTryChangeTrafficLight = isFirstTLTest;
        int tempTrafficLightToTest = 0;
        simFrameCounter++;

        if (isFirstTLTest)
        {
            if(trafficLightScores.Count == 0)
            {
                ClearSimCache();
                FinishSim();
                return;
            }
            actualTestCounter = 0;  // Reset TestCounter

            int oldTLId = GetMaxScoreIndexFromTLScoreDict();    // Set Actual Highest Score ID from TL
            if(oldTLId != 0)
            {
                oldTLHighestScore = trafficLightScores[oldTLId];        // Set the score to check in next run if better
                trafficLightsToTest.Add(oldTLId);

                //Set up corresponding TLs to check if other light gives better result
                correspondingTLID = GetTrafficLightRefFromID(oldTLId).correspondingTLID;
                try
                {
                    correspondingTLScore = trafficLightScores[correspondingTLID];
                }
                catch
                {
                    correspondingTLScore = 0;
                }
            }
        }
        else
        {
            tempTrafficLightToTest = trafficLightsToTest[0];
            int actualMaxLocal = 0;
            try
            {
                actualMaxLocal = trafficLightScores[tempTrafficLightToTest];
            } 
            catch {}

            if(debug)       //DEBUG
            {
                bool picked = actualMaxLocal < oldTLHighestScore;
                var pos = GetTrafficLightRefFromID(tempTrafficLightToTest).transform.position;
                var go = Instantiate(debugTrafficLightPrefab,pos,Quaternion.Euler(0,0,0)).GetComponent<DebugTrafficScoreScript>();
                go.ManualStart("o"+oldTLHighestScore,"s" +actualMaxLocal, picked);

       //         GetTrafficLightRefFromID(tempTrafficLightToTest).ChangeText(oldTLHighestScore + (picked ? "Y":"N"),true);
       //         GetTrafficLightRefFromID(tempTrafficLightToTest).ChangeText(actualMaxLocal.ToString(),false);
            }
            actualTestCounter++;
            if(actualTestCounter == 1)      //First trial of sim traffic light case
            {
                if(actualMaxLocal < oldTLHighestScore)      //Old score was better so change
                {
                    if(GetTrafficLightRefFromID(trafficLightsToTest[0]).state == TrafficLightScript.lightState.green && GetTrafficLightRefFromID(correspondingTLID).state == TrafficLightScript.lightState.green)
                    {
                        int tempSecScore = 0;
                        try
                        {
                            tempSecScore = trafficLightScores[correspondingTLID];
                        }
                        catch 
                        {   
                            Debug.LogError(trafficLightScores.Count + "HART");
                        }

                        addedScoreFirstCheck = actualMaxLocal + tempSecScore;
                        successfullyTestedTL = trafficLightsToTest[0];

                        //if(addedScoreFirstCheck == 0)       //Was perfect trial - no more testing neccesary
                        //{
                        //    RecommendTrafficLight(trafficLightsToTest[0], simFrameCounter);
                        //    trafficLightsToTest.Clear();
                        //}
                        //else
                        //{
                        GetTrafficLightRefFromID(trafficLightsToTest[0]).ChangeText("a" + addedScoreFirstCheck);

                            trafficLightsToTest.Clear();        //Check corresponding TL
                            trafficLightsToTest.Add(correspondingTLID);

                     //   }
                    }
                    else
                    {
                        RecommendTrafficLight(trafficLightsToTest[0], simFrameCounter);
                        trafficLightsToTest.Clear();
                    }
                }
                else
                {
                    // When Score not better after change - ignore Light and start next sim
                    lightsToIgnore.Add(trafficLightsToTest[0]);
                    trafficLightsToTest.Clear();
                    ClearSimCache();
                    StartCoroutine(ContinueSimAfterFrame());        
                    return;
                }
            }
            else if(actualTestCounter == 2)     //after Second light tested
            {
                int tempCorresScores = 0;
                try
                {
                    tempCorresScores = trafficLightScores[successfullyTestedTL];
                } catch { }
                int secondAddedScore = tempCorresScores + actualMaxLocal;

                if (secondAddedScore < addedScoreFirstCheck)
                {
                    RecommendTrafficLight(successfullyTestedTL);
                }
                else
                {
                    RecommendTrafficLight(trafficLightsToTest[0]);
                }
                trafficLightsToTest.Clear();
            }
        }

        ClearSimCache();

            if(trafficLightsToTest.Count > 0)
            {
                StartCoroutine(ContinueSimAfterFrame());
            }
            else
            {
                FinishSim();
            }
    }

    int GetTLIDFromMaxScore ()
    {
        int tempHighestScore = 0;
        int tempTLID = 0;

        foreach (TrafficLightScript tl in simTrafficLights)
        {
            if(tl.GetSimScore() > tempHighestScore)
            {
                tempHighestScore = tl.GetSimScore();
                tempTLID = tl.trafficLightID;
            }
        }
        if (tempTLID == 0)
        {
            Debug.Log("No traffic light with score was found");
        }
        return tempTLID;
    }
    Dictionary <int,int> GetTLScoresAsDictionary ()
    {
        Dictionary<int, int> tempDict = new Dictionary<int, int>();

        Debug.Log("Length: " + simTrafficLights.Count + ", length Dict: " + tempDict.Keys.Count);

        foreach (TrafficLightScript tl in simTrafficLights)
        {
            try
            {
                tempDict.Add(tl.trafficLightID, tl.GetSimScore());
            }
            catch
            {
                Debug.Log("TrafficLightAlreadyInDict: TLID = " + tl.trafficLightID + " + " + tl.simState.ToString(), tl);
            }
        }
        return tempDict;
    }
    void RecommendTrafficLight (int id, int simCounter = 0)
    {
        StartCoroutine(ChangeTrafficLight(id, simCounter));
     //  GetTrafficLightRefFromID(id).LightClicked();
    }

    IEnumerator ChangeTrafficLight (int trafficLightID, int simFrameCounter)
    {
    //    yield return new WaitForSecondsRealtime(reactionTime-(simFrameCounter * .06f));
        yield return new WaitForSecondsRealtime(reactionTime);
        GetTrafficLightRefFromID(trafficLightID).LightClicked();
    }

    int GetMaxScoreIndexFromTLScoreDict()
    {
        int maxTrafficLight = 0;

        if (trafficLightScores.Count >= 2)
        {
            int temp = trafficLightScores.Values.Max();
            int tempIndex = trafficLightScores.Values.ToList().IndexOf(temp);
            maxTrafficLight = trafficLightScores.Keys.ElementAt(tempIndex);
        }
        else if (trafficLightScores.Count == 1)
        {
            maxTrafficLight = trafficLightScores.Keys.ElementAt(0);
        }
        return maxTrafficLight;
    } 





    IEnumerator ContinueSimAfterFrame ()
    {
        yield return null;
        StartSim();
    }

    void SetScoreOfActualSim()
    {
        durationMap.Add(trafficLightsToTest[trafficLightsToTest.Count - 1], simCounter);
        trafficLightsToTest.RemoveAt(trafficLightsToTest.Count-1);
    }

    void FinishSim ()
    {
        int cou = 0;
        foreach (int i in changeTrafficLights)
        {
            cou++;
        //    GetTrafficLightRefFromID(i).ChangeText(cou.ToString());
        }

        // Clear old Data


        inNotTryChangeTrafficLight = true;
        oldTLHighestScore = 0;
        checkedTL.Clear();
        trafficLightsToTest.Clear();
        simFrameCounter = 0;
        lightsToIgnore.Clear();

        if(!debug)
        {
            simEvents.ForEach(delegate (SimEvent simE)
            {
                Destroy(simE.gameObject);
            });
        }
        simEvents.Clear();



        StartCoroutine(StartSimCoroutine());

    }

    public TrafficLightScript GetTrafficLightRefFromID (int id)
    {
        try
        {
            return GameManager.GM.trafficLightsDict[id];    
        } 
        catch {return null;}
    }

    void ClearSimCache()
    {


        foreach (SimulatedParent sp in simObjects)
        {
            if (sp != null)
            {
                Destroy(sp.gameObject);
            }
        }

        trafficLightScores.Clear();
        actualSimRepeats = 0;
        IdPriorityMap.Clear();
        simObjects.Clear();
        simCars.Clear();
        simTrafficLights.Clear();
        simTrafficLightsDict.Clear();
        carSpawnerList.Clear();
        orderOfSpawnCarLocSim.Clear();
        carSpawnTimesSim.Clear();
        boatSpawnTimesSim.Clear();
        emergencySpawnTimesSim.Clear();
        toDeleteList.Clear();
        lightsToChangeInThisRun.Clear();
        nextCarEmergency = false;
        simScore = 0;
        simCounter = 0f;
        trafficLightCounter = 0f;
    }

    public void AddCrash (SpotSimScript spotRef, int priority, CarControlScript carRef)
    {
       
        foreach (TrafficLightScript tl in spotRef.trafficLights)
        {
            if (tl.stateAfterOrange == TrafficLightScript.lightState.red && tl.state == TrafficLightScript.lightState.orange)
            { }
            else if (tl.state == TrafficLightScript.lightState.red)
            { }
            else
            {
              //  AddScoreToTrafficLight(tl.trafficLightID, priority*100);
            }

        } 

    //    if (GameManager.GM.spotsInGame.TryGetValue(spotRef.id, out tempSpotRef))
    //        tempSpotRef.ChangeText(i.ToString());
    }

    public void AddScoreToTrafficLight (int id, int amount, string debugText = "emty")
    {
        GetSimTLFromID(id).AddScoreToTL(amount, debugText);

        //if(id == 0)
        //{
        //    Debug.Log("0-ID");
        //    return 0;
        //}
        //if (lightsToIgnore.Contains(id))
        //    return 0;

        //int i = 0;
        //try
        //{
        //    if (trafficLightScores.TryGetValue(id, out i))
        //    {
        //        i += amount;
        //        trafficLightScores[id] = i;
        //    }
        //    else
        //    {
        //        i = amount;
        //        trafficLightScores.Add(id, i);
        //    }
        //}
        //catch { }

     //       Debug.Log("AddScoreFailed from id: " + id + "   amount: " + amount);
     //       return 0; 
        
    }
    public TrafficLightScript GetSimTLFromID (int id)
    {
        try
        {
            return simTrafficLightsDict[id];
        }
        catch
        {
            return null;
        }
    }

    public void StartSim()
    {
        //if(lightsToIgnore.Count == 0)
        //{
        //    foreach (TrafficLightScript tl in GameManager.GM.trafficLights)     // Clear debug text
        //    {
        //     //   tl.ChangeText("", true);
        //     //   tl.ChangeText("", false); ;

        //        tl.DebugListEntries.Clear();
        //    }
        //}
         

        SimulatedParent[] copyObj = FindObjectsOfType<SimulatedParent>();
        foreach (SimulatedParent sp in copyObj)
        {
            CarInFrontDetect CarInFrontCheck;
            BoatColliderScript boatColScript;


            if (sp.TryGetComponent<CarInFrontDetect>(out CarInFrontCheck))      // Car in Front Obj was created twice!
            { }
            else if (sp.TryGetComponent<BoatColliderScript>(out boatColScript)) //No spawn of additional boatCollider
            { }

            else
            {
                SimulatedParent tempRef = Instantiate(sp, sp.transform.position + new Vector3(0, offsetFromOriginal, 0), sp.transform.rotation);
                simObjects.Add(tempRef);
                tempRef.simState = SimulatedParent.simulationState.simulated;

                //DestroySpriteRenderers
                if (destroyRenderers)
                {
                    SpriteRenderer sr;
                    if (tempRef.gameObject.TryGetComponent<SpriteRenderer>(out sr))
                        Destroy(sr);
                    try
                    {
                        Destroy(tempRef.gameObject.GetComponentInChildren<SpriteRenderer>());
                    }
                    catch { }
                }

                try         // Check if is car and then give probs over
                {
                    CarControlScript car = (CarControlScript)tempRef;
                    CarControlScript copyCar = (CarControlScript)sp;

                    //Set settings of detect in front obj.
                    CarInFrontDetect tempCarInFront = car.GetComponentInChildren<CarInFrontDetect>();
                    tempCarInFront.simState = SimulatedParent.simulationState.simulated;
                    simObjects.Add(tempCarInFront);
                    //
                    simCars.Add(car);
                    car.state = copyCar.state;
                    car.timeCounter = copyCar.timeCounter;
                    car.carSpeed = copyCar.carSpeed;
                    car.actualWaitingLightID = copyCar.actualWaitingLightID;
                }
                catch { }

                try
                {
                    BridgeScript bridge = (BridgeScript)tempRef;

                    BoatColliderScript tempBoatCol = bridge.GetComponentInChildren<BoatColliderScript>();
                    tempBoatCol.simState = SimulatedParent.simulationState.simulated;
                    simObjects.Add(tempBoatCol);
                }
                catch { }

                try         // Check if is trafffic light
                {
                    TrafficLightScript light = (TrafficLightScript)tempRef;
          //          light.waitingCarsCounter = GetTrafficLightRefFromID(light.trafficLightID).trafficLightID;
                    simTrafficLights.Add(light);
                    simTrafficLightsDict.Add(light.trafficLightID, light);
                }
                catch { }

                try         // Check if is boatSpawner
                {
                    BoatSpawnerScript boatSpawn = (BoatSpawnerScript)tempRef;
                    boatSpawnerSim = boatSpawn;
                }
                catch { }

                try         // Check if is carSpawner
                {
                    CarSpawner carSpawn = (CarSpawner)tempRef;
                    carSpawnerList.Add(carSpawn);
                }
                catch { }
            }
        }
        // Assign Data from GameManager
        SetStuffFromGameManager();

        //Add traffic lights to change in Simulation
        if (changeTrafficLights.Count > 0)
        {
            foreach (int tlID in changeTrafficLights)
            {
                foreach (TrafficLightScript tlRef in simTrafficLights)
                {
                    if (tlRef.trafficLightID == tlID)
                    {
                        lightsToChangeInThisRun.Add(tlRef);
                        break;
                    }
                }
            }
        }

        if (trafficLightsToTest.Count > 0)      //Add the traffic light ref of the actual test Light
        {
            int tempID = trafficLightsToTest[trafficLightsToTest.Count - 1];
            foreach (TrafficLightScript tlRef in simTrafficLights)
            {
                if (tlRef.trafficLightID == tempID)
                {
                    lightsToChangeInThisRun.Add(tlRef);
                    break;
                }
            }
        }

        foreach (SimulatedParent sp in simObjects)      // init Sim of all objects
        {

            if (sp != null)
            {
                sp.InitSimulation();
            }
            else
            {
                toDeleteList.Add(sp);
            }
        }

        StartCoroutine(WaitOneFrame());
    }
        

    void SetStuffFromGameManager ()
    {
        scoreRef = Score.sc;
        simScore = scoreRef.GetScore();     // get actual score

        simTimeCounter = GameManager.GM.timeCounter;
        carSpawnTimesSim = new List<float> (GameManager.GM.carSpawnTimes);
        orderOfSpawnCarLocSim = new List<int> (GameManager.GM.orderOfSpawnCarLoc);
        emergencySpawnTimesSim = new List<float> (GameManager.GM.emergencySpawnTimes);
        boatSpawnTimesSim = new List<float>(GameManager.GM.boatSpawnTimes);

        List<CarSpawner> tempCarSpawn = new List<CarSpawner>();

        foreach (CarSpawner cs in GameManager.GM.carSpawners)
        {
            foreach (CarSpawner csSim in carSpawnerList)
            {
                if (cs.PathID == csSim.PathID)
                {
                    tempCarSpawn.Add(csSim);
                }
            }
        }
        carSpawnerList = tempCarSpawn;
    }

    IEnumerator delayUpdate()
    {
        yield return new WaitForSecondsRealtime(simulationSteps);
        UpdateFunc();

        StartCoroutine(delayUpdate());
    }

    void UpdateFunc ()
    {
        foreach(SimulatedParent sp in simObjects)
        {
            if(sp != null)
            {
                sp.UpdateSimulation(simulationSteps);
            }
            else
            {
                try
                {
                    toDeleteList.Add(sp);
                } catch {}
            }
        }

        if(toDeleteList.Count > 0)
        {
            DoDeleteJob();
        }

        simCounter += simulationSteps;

        CheckIfSwitchTrafficLight();
        SpawnHandle();

        Physics2D.SyncTransforms();     // Ich liebe dich
    }

    void CheckIfSwitchTrafficLight ()       // Switch the light and delete from lightsToChangeThisRun
    {
        if (lightsToChangeInThisRun.Count > 0)
        {
            trafficLightCounter += simulationSteps;

            if (trafficLightCounter >= reactionTime)
            {
                trafficLightCounter = 0f;
                lightsToChangeInThisRun[lightsToChangeInThisRun.Count - 1].LightClicked();
                lightsToChangeInThisRun.RemoveAt(lightsToChangeInThisRun.Count - 1);
            }
        }
    }

    void DoDeleteJob ()
    {
        foreach (SimulatedParent sp in toDeleteList)
        {
            try
            {
                simObjects.Remove(sp);
            } 
            catch { UnityEngine.Debug.Log("Failed to remove SimObj"); }
        }

        try
        {
            toDeleteList.Clear();
        }
        catch {}
    }

    void SpawnHandle ()
    {
        simTimeCounter += simulationSteps;
        try
        {
            if (carSpawnTimesSim[0] <= simTimeCounter)      //Check if spawn car
            {
                if (!GameManager.GM.spawnEmergency)     //CheckIfSpawnEmegency
                    nextCarEmergency = false;

                carSpawnerList[orderOfSpawnCarLocSim[0]].SpawnCar(nextCarEmergency);
                if (nextCarEmergency)
                    nextCarEmergency = false;
                orderOfSpawnCarLocSim.RemoveAt(0);
                carSpawnTimesSim.RemoveAt(0);
            }
        }
        catch { }
        //try
        //{
        //    if (boatSpawnTimesSim[0] <= simTimeCounter)
        //    {
        //            boatSpawnerSim.SpawnBoatFunc();
        //            boatSpawnTimesSim.RemoveAt(0);
        //    }
        //    } 
        //catch { Debug.Log("ErrorHere1"); }
        //try
        //{
        //    if (emergencySpawnTimesSim[0] <= simTimeCounter)
        //    {
        //        nextCarEmergency = true;
        //        emergencySpawnTimesSim.RemoveAt(0);
        //    }
        //}
        //catch { Debug.Log("ErrorHere2"); }
    }

    IEnumerator WaitOneFrame()
    {
        if (debug)
        {
            foreach (GameObject debugg in debugTrafficLightsScores)      //DEBUG!
            {
                Destroy(debugg);
            }
            debugTrafficLightsScores.Clear();



            foreach (TrafficLightScript tl in simTrafficLights)
            {
                int tempScore = 0;
                if (trafficLightScores.TryGetValue(tl.trafficLightID, out tempScore))
                {
                    GameObject debugTrafL = Instantiate(debugTrafficLightPrefab, tl.transform.position, Quaternion.Euler(0, 0, 0));
                    debugTrafficLightsScores.Add(debugTrafL);
                    if(trafficLightsToTest.Count == 0)
                        debugTrafL.GetComponent<DebugTrafficScoreScript>().ManualStart(tempScore.ToString(), "");
                    else
                        debugTrafL.GetComponent<DebugTrafficScoreScript>().ManualStart("", tempScore.ToString());

                }
            }
        }

        yield return null;
        actualSimRepeats++;

        for (int i = 0; i < simulationStepsAmount; i++)
        {
            UpdateFunc();
        }
        if (actualSimRepeats >= simulationStepRepeat)
        {
            try
            {
                FinishPart();
            }
            catch
            { 
                Debug.Log("FICK");
                FinishPart ();
            }
        }
        else
            StartCoroutine(WaitOneFrame());
    }

    IEnumerator OneSecDelay ()
    {
        yield return new WaitForSecondsRealtime((simulationSteps * simulationStepsAmount) /timeScale);
        for (int i = 0; i < simulationStepsAmount; i++)
        {
            UpdateFunc();
        }
        StartCoroutine(OneSecDelay());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            StartSim();
        if(Input.GetKeyDown(KeyCode.T))
        {
            foreach (TrafficLightScript tl in simTrafficLights)
            {
                tl.LightClicked();
            }
        }
    }
}
