using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Adjust")]
    [SerializeField] string experimentSettingsName;

    public static GameManager GM;

    static public float set_TimeScale;
    static public float set_AutoAmount;
    static public float set_EmergencyAmount;
    static public float set_BoatAmount;
    static public float set_Duration;
    [HideInInspector] public bool canGameStart = false;
    [HideInInspector] public List<TrafficLightScript> trafficLights = new List<TrafficLightScript>();
    [HideInInspector] public Dictionary<int,TrafficLightScript> trafficLightsDict = new Dictionary<int,TrafficLightScript>();
    public List<CarSpawner> carSpawners = new List<CarSpawner>();
    [HideInInspector] public List<BoatSpawnerScript> boatSpawner = new List<BoatSpawnerScript>();

    public List<int> orderOfSpawnCarLoc = new List<int>();

    public bool spawnEmergency = true;

    [Header("To assign")]
    [SerializeField] Canvas mainCanvasRef; 
    [SerializeField] Text InfoTextRef;

    //Time and spawning
    [Space]
    public float startTime;
    public float timeCounter = 0;
    public List<float> carSpawnTimes = new List<float>();
    public List<float> boatSpawnTimes = new List<float>();
    public List<float> emergencySpawnTimes = new List<float>();

    public Dictionary<int, SpotSimScript> spotsInGame = new Dictionary<int, SpotSimScript>();

    // Private Vars
    int actualTimerNum = 4;
    private Camera camRef;
    private bool nextCarEmergency = false;
   
    string postSettingsURL = "https://carcontroldatabase.000webhostapp.com/PHP/LoadSettingsFromUnity.php";

    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
            AwakeGameManager();
        }
        else
            Destroy(gameObject);


    }
    private void AwakeGameManager()
    {
        LoadSettings();
        AssignSpawerIDs();
    }

    void AssignSpawerIDs ()         // + Set up References tp the gameObjects
    {
        CarSpawner[] tempCarSpawners = GameObject.FindObjectsOfType<CarSpawner>();
        int counter = 0;
        foreach (CarSpawner cs in tempCarSpawners)
        {
            carSpawners.Add(cs);

            counter++;
            cs.PathID = counter;
        }

        CarContinueLane[] tempCarContinues = GameObject.FindObjectsOfType<CarContinueLane>();
        counter ++;
        foreach (CarContinueLane cc in tempCarContinues)
        {
            counter++;
            cc.PathID = counter;
        }

        TrafficLightScript[] tempTrafficLights = FindObjectsOfType<TrafficLightScript>();       //Assign Traffic lights
        counter = 1;
        foreach (TrafficLightScript tl in tempTrafficLights)
        {
            counter++;
            tl.trafficLightID = counter;
            trafficLights.Add(tl);
            trafficLightsDict.Add(counter, tl);
        }

        //Assign Spot ID
        SpotSimScript[] tempSpots = FindObjectsOfType<SpotSimScript>();
        counter = 1;
        foreach(SpotSimScript sp in tempSpots)
        {
            counter++;
            spotsInGame.Add(counter, sp);
            sp.id = counter;
        }

        BoatSpawnerScript[] tempBoatSpawner= FindObjectsOfType<BoatSpawnerScript>();       //Assign BoatSpawners
        foreach (BoatSpawnerScript bs in tempBoatSpawner)
        {
            boatSpawner.Add(bs);
        }

        int amountSpawnerCar = carSpawners.Count;     //Set up Random Order to spawn cars

        for (int i = 0; i < 10000; i++)
        {
            orderOfSpawnCarLoc.Add(Random.Range(0, amountSpawnerCar));
        }
    }
    void SetSpawnLists ()           //SetUpLists when to spawn what
    {
        float tempDelay = 60 / set_AutoAmount;
        for (int i = 1; i<set_Duration * set_AutoAmount * 1.1f;i++)
        {
            carSpawnTimes.Add(tempDelay * i);
        }

        tempDelay = 60 / set_BoatAmount;

        for (int i = 0; i<set_Duration * set_BoatAmount * 1.5f;i++)
        {
            float t = Random.Range(0f, 30 / set_BoatAmount);
            boatSpawnTimes.Add(t + (tempDelay * i));
        }

        tempDelay = 60 / set_EmergencyAmount;

        for (int i = 0; i<set_Duration * set_EmergencyAmount * 1.5f;i++)
        {
            float e = Random.Range(0f, 30 / set_EmergencyAmount);
            emergencySpawnTimes.Add(e + (tempDelay * i));
        }
    }
        


    private void LoadSettings()
    {
        StartCoroutine(LoadSettingsFromServer());
    }

    IEnumerator LoadSettingsFromServer()    //Load the settings
    {
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("name", experimentSettingsName));
        UnityWebRequest getSettingsRequest = UnityWebRequest.Post(postSettingsURL, formData);

        yield return getSettingsRequest.SendWebRequest();

        if (getSettingsRequest.result == UnityWebRequest.Result.ConnectionError || getSettingsRequest.result == UnityWebRequest.Result.ProtocolError || getSettingsRequest.result == UnityWebRequest.Result.DataProcessingError)
        {
            Debug.Log("Error: " + getSettingsRequest.error);
            InfoTextRef.text = ("Bitte überpfüfe deine Internetverbindung");
            StartCoroutine(WaitASec(false));
        }
        else
        {
            string tempText = getSettingsRequest.downloadHandler.text;
            Debug.Log(getSettingsRequest.downloadHandler.text);
            string[] cutString = tempText.Split('|');

            float notUsedFloat;
            if (float.TryParse(cutString[0], out notUsedFloat) && float.TryParse(cutString[1], out notUsedFloat))
            {
                for (int i = 0; i < cutString.Length; i++)      // assign the Settings data
                {
                    switch (i)
                    {
                        case 0:
                            set_TimeScale = float.Parse(cutString[i]);
                            break;
                        case 1:
                            set_AutoAmount = float.Parse(cutString[i]);
                            break;
                        case 2:
                            set_EmergencyAmount = float.Parse(cutString[i]);
                            break;
                        case 3:
                            set_BoatAmount = float.Parse(cutString[i]);
                            break;
                        case 4:
                            set_Duration = float.Parse(cutString[i]);
                            break;
                    }
                }
            }
            else
            {
                StartCoroutine(WaitASec(false));    // Wrong message came back!
                InfoTextRef.text = "Falsche Server antwort. Es wird nochmal versucht";
            }



            InfoTextRef.text = "Spiel startet in";
            StartCoroutine(WaitASec(true));
        }
    }

    void StartGame ()
    {
        camRef = Camera.main;
        canGameStart = true;
        startTime = Time.realtimeSinceStartup;
        mainCanvasRef.gameObject.SetActive(false);
        SetSpawnLists();
            /*
        StartCoroutine(SpawnCars());        //Start Car Spawning & Boat spawning
        StartCoroutine(SpawnBoatFixStep());
        StartCoroutine(SpawnBoatRandomized());
        StartCoroutine(SpawnEmergencyFixStep());
        StartCoroutine(SpawnEmergencyRandomized());
            */
    }

    /*IEnumerator SpawnCars()
    {
        yield return new WaitForSecondsRealtime(60/set_AutoAmount);

        if(canGameStart)            // Spawn one car and Remove one From Entry
        {
            carSpawners[orderOfSpawnCarLoc[0]].SpawnCar(nextCarEmergency);
            if (nextCarEmergency)
                nextCarEmergency = false;
            orderOfSpawnCarLoc.RemoveAt(0);

            StartCoroutine(SpawnCars());
        }
    }*/
         
    IEnumerator SpawnBoatFixStep()      //BoatSpawner control
    {
        yield return new WaitForSecondsRealtime(60 / set_BoatAmount);

        StartCoroutine(SpawnBoatRandomized());
        StartCoroutine(SpawnBoatFixStep());

    }   
        
    IEnumerator SpawnBoatRandomized()
    {
        float t = Random.Range(0f, 30 / set_BoatAmount);
        yield return new WaitForSecondsRealtime(t);

        boatSpawner[0].SpawnBoatFunc();
    }

    IEnumerator SpawnEmergencyFixStep()      //Emergency Car variable
    {
        yield return new WaitForSecondsRealtime(60 / set_EmergencyAmount);

        StartCoroutine(SpawnEmergencyRandomized());
        StartCoroutine(SpawnEmergencyFixStep());
    }

    IEnumerator SpawnEmergencyRandomized()
    {
        float t = Random.Range(0f, 30 / set_EmergencyAmount);
        yield return new WaitForSecondsRealtime(t);

        nextCarEmergency = true;
    }



    IEnumerator WaitASec(bool isTimer)
    {
        yield return new WaitForSeconds(1);     
        if(isTimer)
        {
            if (actualTimerNum == 1)
            {
                StartGame();
            }
            else
            {
                actualTimerNum--;
                InfoTextRef.text = actualTimerNum.ToString();
                StartCoroutine(WaitASec(true));
            }
        }
        else                    // Loading not successful - retry
        {
            StartCoroutine(LoadSettingsFromServer());
    
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            try
            {
                Vector3 mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (hit.collider != null)
                {
                    if(hit.collider.CompareTag("TrafficLightClick"))
                    {
                        TrafficLightScript trafLight = hit.collider.gameObject.GetComponentInParent<TrafficLightScript>();
                        trafLight.LightClicked();
                    }                
                }
            } 
            catch { }
        }
        if(canGameStart)
        {

            //timeCounter = Time.realtimeSinceStartup - startTime;
            timeCounter += Time.deltaTime;

            if(carSpawnTimes[0]<= timeCounter)      //Check if spawn car
            {
                if (!spawnEmergency)        //SpawnNoEmergency
                    nextCarEmergency = false;

                carSpawners[orderOfSpawnCarLoc[0]].SpawnCar(nextCarEmergency);
                if (nextCarEmergency)
                    nextCarEmergency = false;
                orderOfSpawnCarLoc.RemoveAt(0);
                carSpawnTimes.RemoveAt(0);
            }
            if(boatSpawnTimes[0]<= timeCounter)
            {
                try
                {
                    boatSpawner[0].SpawnBoatFunc();
                } catch { }
                boatSpawnTimes.RemoveAt(0);
            }
            if(emergencySpawnTimes[0] <=timeCounter)
            {
                nextCarEmergency = true;
                emergencySpawnTimes.RemoveAt(0);
            }
        }
    }
}
