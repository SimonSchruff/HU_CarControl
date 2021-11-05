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

    bool wasCrash = false;

   // public List<int> trafficLightsToChangeIDs = new List<int>();

    //Traffic light prioritize
    int oldTLHighestScore = 0;
    int simFrameCounter = 0;

    List<int> checkedTL = new List<int>();
    Dictionary<int, int> copyOfStartScores = new Dictionary<int, int>();

    //ToDeleteList
    public List<SimulatedParent> toDeleteList = new List<SimulatedParent>();
    private void Awake()
    {
        if (sim == null)
        {
            sim = this;
        }
        else
            Destroy(this.gameObject);


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

    void FinishPart_Old()
    {
        bool finished = false;
        bool isInitSituation = false;

        if (trafficLightsToTest.Count >= 1)
        {
            SetScoreOfActualSim();
        }

        isInitSituation = trafficLightsToTest.Count == 0;

        if (isInitSituation)
        {
            if (durationMap.Count >= 1)
            {
                FindHighestScoreAndAssign();
            }

            if (actualDepth < simulationDepth)
            {
                // Go One Layer deeper in simulation
           //     if (actualSpotRef != null)
          //      {
           //         foreach (TrafficLightScript tls in actualSpotRef.trafficLights)
           //         {
         //              trafficLightsToTest.Add(tls.trafficLightID);
         //           }
          //          actualDepth++;
          //      }
              //  else    // When no ref given
              //  {
              //      FinishSim();
             //       finished = true;
             //   }
            }
            else
            {
                FinishSim();
                finished = true;
            }
        }
        else    //What to do when multiple cases are tested
        {
        }


        ClearSimCache();


        if (!finished)
        {
            if (isInitSituation)
            {
                if (actualDepth <= simulationDepth)
                {
                    StartCoroutine(ContinueSimAfterFrame());
                }
                else
                {
                    FinishSim();
                }
            }
            else
            {
                StartCoroutine(ContinueSimAfterFrame());
            }
        }
    }

    void WriteScores ()
    {
        if(SimulationControlScript.sim.debug)
        {
            foreach(TrafficLightScript tl in simTrafficLights)
            {
                tl.ChangeText(tl.waitingCarsCounter+"");
            }
        }
    }


    void FinishPart()
    {
        bool isFirstTLTest = trafficLightsToTest.Count == 0;
        inNotTryChangeTrafficLight = isFirstTLTest;
        int tempTrafficLightToTest = 0;
        simFrameCounter++;

        if(simFrameCounter == 1)
            WriteScores();

        if (isFirstTLTest)
        {
            if(trafficLightScores.Count == 0)
            {
                ClearSimCache(isFirstTLTest);
                FinishSim();
                return;
            }

         //   if (trafficLightTestCounter == 0)
        //        copyOfStartScores = new Dictionary<int, int>(trafficLightScores);

            int oldTLId = GetMaxScoreIndexFromTLScoreDict();
            if(oldTLId != 0)
            {
                oldTLHighestScore = trafficLightScores[oldTLId];
                trafficLightsToTest.Add(oldTLId);
            }
        }
        else
        {
            tempTrafficLightToTest = trafficLightsToTest[0];
            int actualMaxLocal = 0;
            try
            {
                actualMaxLocal = trafficLightScores[trafficLightsToTest[0]];
            } 
            catch {}

            if(debug)       //DEBUG
            {
                bool picked = actualMaxLocal < oldTLHighestScore;
                var pos = GetTrafficLightRefFromID(tempTrafficLightToTest).transform.position;
                var go = Instantiate(debugTrafficLightPrefab,pos,Quaternion.Euler(0,0,0)).GetComponent<DebugTrafficScoreScript>();
                go.ManualStart("o"+oldTLHighestScore,"s" +actualMaxLocal, picked);

                GetTrafficLightRefFromID(tempTrafficLightToTest).ChangeText(oldTLHighestScore + (picked ? "Y":"N"),true);
                GetTrafficLightRefFromID(tempTrafficLightToTest).ChangeText(actualMaxLocal.ToString(),false);
            }

            if(actualMaxLocal < oldTLHighestScore)      //Old score was better so change
            {
                RecommendTrafficLight(trafficLightsToTest[0], simFrameCounter);


                trafficLightsToTest.Clear();
            }
            else
            {
                // When Score not better after change - ignore Light and start next sim
                lightsToIgnore.Add(trafficLightsToTest[0]);
                trafficLightsToTest.Clear();
                ClearSimCache(isFirstTLTest, tempTrafficLightToTest);
                StartCoroutine(ContinueSimAfterFrame());        
                return;


                try
                {
                    copyOfStartScores.Remove(trafficLightsToTest[0]);
                } catch { Debug.Log("Delete from Copy start scores failed", gameObject); }

                int oldTLId = GetMaxScoreIndexFromTLScoreDict();        // Add Emtry tp TestLights
                if (oldTLId != 0)
                {
                    oldTLHighestScore = copyOfStartScores[oldTLId];
                    trafficLightsToTest.Add(oldTLId);
                }
            }
        }

        ClearSimCache(isFirstTLTest, tempTrafficLightToTest);

   //     if(trafficLightTestCounter > 1)
  //      {
   //         FinishSim();
           // Debug.Log("Finished Cause counter");
   //     }
  //      else
   //     {
            if(trafficLightsToTest.Count > 0)
            {
                StartCoroutine(ContinueSimAfterFrame());
            }
            else
            {
                FinishSim();
            }
  //      }
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

    void FindHighestScoreAndAssign ()
    {
        int maxTrafficLight = 0;

        if (durationMap.Count >= 2)
        {
            float temp = durationMap.Values.Max();
            int tempIndex = durationMap.Values.ToList().IndexOf(temp);
            maxTrafficLight = durationMap.Keys.ElementAt(tempIndex);
        //    Debug.LogError(maxTrafficLight);
        }
        else if (durationMap.Count == 1)
        {
            maxTrafficLight = durationMap.Keys.ElementAt(0);
        }
        else
        {
            FinishSim();
            return;
        }

        changeTrafficLights.Add(maxTrafficLight);
        durationMap.Clear();
    }

    void ClearSimCache(bool isFirstText = false, int testedTrafficLightID = 0)
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
                AddScoreToTrafficLight(tl.trafficLightID, priority*100);
            }

        } 

    //    if (GameManager.GM.spotsInGame.TryGetValue(spotRef.id, out tempSpotRef))
    //        tempSpotRef.ChangeText(i.ToString());
    }

    public int AddScoreToTrafficLight (int id, int amount)
    {
        if(id == 0)
        {
            Debug.Log("0-ID");
            return 0;
        }
        if (lightsToIgnore.Contains(id))
            return 0;

        int i = 0;
        try
        {
            if (trafficLightScores.TryGetValue(id, out i))
            {
                i += amount;
                trafficLightScores[id] = i;
            }
            else
            {
                i = amount;
                trafficLightScores.Add(id, i);
            }
        }
        catch { }
        try
        {
            GetTrafficLightRefFromID(id).ChangeText(i.ToString());      // Debug
        }
        catch { }
        return i;
     //       Debug.Log("AddScoreFailed from id: " + id + "   amount: " + amount);
     //       return 0; 
        
    }

    public void StartSim()
    {
        if(lightsToIgnore.Count == 0)
        {
            foreach (TrafficLightScript tl in GameManager.GM.trafficLights)     // Clear debug text
            {
                tl.ChangeText("", true);
                tl.ChangeText("", false); ;

                tl.DebugListEntries.Clear();
            }
        }
         

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

        /*List<CarControlScript> carList = new List<CarControlScript>();
        foreach (CarControlScript ccs in simCars)
        {
            if (ccs == null)
            {
                CarCo
            }
        }
        */
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
        if (boatSpawnTimesSim[0] <= simTimeCounter)
        {
            try
            {
                boatSpawnerSim.SpawnBoatFunc();
            } catch { }
            boatSpawnTimesSim.RemoveAt(0);
        }
        if (emergencySpawnTimesSim[0] <= simTimeCounter)
        {
            nextCarEmergency = true;
            emergencySpawnTimesSim.RemoveAt(0);
        }
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

        bool stop = false;

        for (int i = 0; i < simulationStepsAmount; i++)
        {
            if (!wasCrash)
                UpdateFunc();
            else
            {
                wasCrash = false;
                FinishPart();
                stop = true;
                break;
            }
        }
        if (actualSimRepeats >= simulationStepRepeat)
            FinishPart();
        else if (!stop)
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
