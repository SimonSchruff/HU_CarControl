using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    public float trialDurationInMin = .5f;
    [Header("Adjust")]
    public float spawnDelay= 1f;
    public float gridSize = 1.5f;

    [SerializeField] int SpawnCarAmountPerTurn = 2;
    [SerializeField] int SpawnCarAmountAtBegin = 5;

    [SerializeField] Camera camRef;
    [SerializeField] ContactFilter2D filter;
    [SerializeField] LayerMask mask;

    bool gameRunning = false;
    float timeCounter = 0f;
    bool isInit = false;

    [SerializeField] bool isLastLevel = false;

    [Header("Gameplay not change")]
    public int actualStep = 0;

    public SaveTrialClass actualSaveClass;
    public List<SaveTrialClass> allSaveClasses = new List<SaveTrialClass>();

    public Crosses[] crossesRefs;
    public CarSpawnScript[] carSpawnRefs;

    public static Control instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame(string TrialName = "trial_test", bool safeData = true, string assistanceLevel = "x")
    {
        crossesRefs = FindObjectsOfType<Crosses>();
        carSpawnRefs = FindObjectsOfType<CarSpawnScript>();
        
       // StartCoroutine(UpdateFunc(true));
        StartCoroutine(FinishTrial());

        gameRunning = true;
        isInit = true;

        if (SQLSaveManager.instance)
        {
            actualSaveClass = SQLSaveManager.instance.gameObject.AddComponent<SaveTrialClass>();
            actualSaveClass.trialName = TrialName;
            actualSaveClass.assistance = assistanceLevel;
            actualSaveClass.saveData = safeData;
            // actualSaveClass.assistance = AssistanceSelectScript.assiSel.actualAssiSelect.ToString();
        }
        else
        {
            Debug.Log("No Save Manager found; Trial will not be saved! Control.cs : 68");
        }

        if (AssistanceSelectScript.instance.assiSelectState == AssistanceSelectScript.AssiSelectStates.Area)
        {
            actualSaveClass.percentageArea = 1;
        }
        else if (AssistanceSelectScript.instance.assiSelectState == AssistanceSelectScript.AssiSelectStates.Specific)
        {
            actualSaveClass.percentageSpecific = 1;
        }
        else if (AssistanceSelectScript.instance.assiSelectState == AssistanceSelectScript.AssiSelectStates.Select)
        {
            actualSaveClass.InitChangeAssistanceActive(SimplAssis.instance.actualAssistance == SimplAssis.AssiState.areaHelp);   
        }
    }

    
    IEnumerator FinishTrial ()
    {
        yield return new WaitForSeconds(trialDurationInMin * 60);
        FinishTrialFunc();
    }

    void FinishTrialFunc ()
    {
        gameRunning = false;
        timeCounter = 0;
        actualSaveClass.score = ScoreSimple.sco.GetScore(); // Set score
        actualSaveClass.FinishTrial();

        ScoreSimple.sco.ResetScore();   //Reset score

        foreach (var cars in FindObjectsOfType<CarSimple>())
        {
            Destroy(cars.gameObject);
        }
        SimplAssis.instance.UpdateAssistance();

       // StopCoroutine(UpdateFunc());

       //Destroy Sec Task
        foreach (var sec in FindObjectsOfType<SecondaryTask>())   
        {
            Destroy(sec.gameObject);
        }
        CheckIfAddLastScoreClass();

        AssistanceSelectScript.instance.ChangeUIVisibility(false); 
        
        // Continue in Survey
        if(FragebogenManager.instance)
            FragebogenManager.instance.NextQuestion();
        else
            print("No FragebogenManager found; Cannot continue; Control.cs : 122");
        
        actualSaveClass = null;
    }   

    void CheckIfAddLastScoreClass()
    {
        if(isLastLevel)
        {
            float tempScore1 = 0;
            float tempScore2 = 0;

            var saveTrials = FindObjectsOfType<SaveTrialClass>();

            foreach (var trial in saveTrials)
            {
                switch (trial.trialName)
                {
                    case "tr2":
                        tempScore1 += trial.score;
                        break;
                    case "tr3":
                        tempScore1 += trial.score;
                        break;
                    case "tr4":
                        tempScore1 += trial.score;
                        break;
                    case "tr5":
                        tempScore2 += trial.score;
                        break;

                    default:
                        break;
                }
            }

            float finScore = ((tempScore1 / 3) + tempScore2) / 2;

            SaveTrialClass tri = SQLSaveManager.instance.gameObject.AddComponent<SaveTrialClass>();
            tri.score = Mathf.RoundToInt(finScore);
            tri.trialName = "total"; // Added Trial Name
            tri.assistance = "total";
        }
    }
    
    /*
    IEnumerator UpdateFunc(bool isStart = false)
    {
        yield return new WaitForSeconds(isStart ? 2 : spawnDelay);

        if (gameRunning)
        {
            UpdateElems();
            SimplAssis.assi.UpdateAssistance();

            StartCoroutine(UpdateFunc());
        }

    }
    */
    
    void UpdateFuncToCall (bool IsInit = false)
    {
        if (gameRunning)
        {
            UpdateElems(IsInit);
            SimplAssis.instance.UpdateAssistance();
        }
    }

    void UpdateElems (bool IsInit = false)
    {
        foreach (var cs in carSpawnRefs)
        {
            cs.UpdateCheckIfSpawnAllowed();
        }

        GetPossibleSpawnerAndSpawnCar(IsInit);

        foreach (Crosses cr in crossesRefs)   
        {
            cr.SimpleUpdate();
            cr.ClearCrosses();
        }
        foreach (var cs in carSpawnRefs)
        {
            cs.SimpleUpdate();
        }

        CarSimple[] tempCarsRef = FindObjectsOfType<CarSimple>();
        foreach (var car in tempCarsRef)
        {
            car.SimpleUpdate();
            car.FillCrosses();
        }
    }
    public void GetPossibleSpawnerAndSpawnCar (bool IsInit = false)
    {
        if (SpawnCarAmountPerTurn == 1)
        {
            List<CarSpawnScript> possibleSpawnPos = new List<CarSpawnScript>();
            foreach (var carSpawn in carSpawnRefs)
            {
                if(carSpawn.AllowSpawnCar)
                {
                    possibleSpawnPos.Add(carSpawn);
                }
            }
            
            CarSpawnScript temp = possibleSpawnPos[Random.Range(0, possibleSpawnPos.Count - 1)];
            temp.SpawnCar();
            possibleSpawnPos.Remove(temp);
        }
        else
        {
            List<CarSpawnScript> horSpawn = new List<CarSpawnScript>();
            List<CarSpawnScript> verSpawn = new List<CarSpawnScript>();

            if (IsInit)
            {
                foreach (var carSpawn in carSpawnRefs)
                {
                    if (carSpawn.AllowSpawnCar && !carSpawn.IsHorizontal)
                    {
                        horSpawn.Add(carSpawn);
                    }
                }
                Debug.Log("XXXXXXXX__" + SpawnCarAmountAtBegin / 2);
                for (int i = 0; i < (SpawnCarAmountAtBegin / 2); i++)
                {
                    if (horSpawn.Count > 0)
                    {
                        CarSpawnScript tempH = horSpawn[Random.Range(0, horSpawn.Count)];
                        verSpawn.Remove(tempH);
                        tempH.SpawnCar();

                        if (tempH.correspondingSpawner != null)
                        {
                            tempH.correspondingSpawner.DisableAllowSpawn();
                        }
                    }
                }

                foreach (var carSpawn in carSpawnRefs)
                {
                    if (carSpawn.AllowSpawnCar && carSpawn.IsHorizontal)
                    {
                        verSpawn.Add(carSpawn);
                    }
                }

                for (int i = 0; i < (SpawnCarAmountAtBegin / 2); i++)
                {
                    if (verSpawn.Count > 0)
                    {
                        CarSpawnScript tempV = verSpawn[Random.Range(0, verSpawn.Count)];
                        verSpawn.Remove(tempV);
                        tempV.SpawnCar();

                    }
                }
            }
            else
            {
                if (Random.Range(0, 2) == 1)
                {
                    foreach (var carSpawn in carSpawnRefs)
                    {
                        if (carSpawn.AllowSpawnCar && !carSpawn.IsHorizontal)
                        {
                            horSpawn.Add(carSpawn);
                        }
                    }
                    if (horSpawn.Count > 0)
                    {
                        CarSpawnScript tempH = horSpawn[Random.Range(0, horSpawn.Count)];
                        tempH.SpawnCar();

                        if (tempH.correspondingSpawner != null)
                        {
                            tempH.correspondingSpawner.DisableAllowSpawn();
                        }
                    }

                    foreach (var carSpawn in carSpawnRefs)
                    {
                        if (carSpawn.AllowSpawnCar && carSpawn.IsHorizontal)
                        {
                            verSpawn.Add(carSpawn);
                        }
                    }
                    if (verSpawn.Count > 0)
                    {
                        CarSpawnScript tempV = verSpawn[Random.Range(0, verSpawn.Count)];
                        tempV.SpawnCar();
                    }
                }
                else
                {
                    foreach (var carSpawn in carSpawnRefs)
                    {
                        if (carSpawn.AllowSpawnCar && carSpawn.IsHorizontal)
                        {
                            verSpawn.Add(carSpawn);
                        }
                    }
                    if (verSpawn.Count > 0)
                    {
                        CarSpawnScript tempV = verSpawn[Random.Range(0, verSpawn.Count)];
                        tempV.SpawnCar();

                        if (tempV.correspondingSpawner != null)
                        {
                            tempV.correspondingSpawner.DisableAllowSpawn();
                        }
                    }

                    foreach (var carSpawn in carSpawnRefs)
                    {
                        if (carSpawn.AllowSpawnCar && !carSpawn.IsHorizontal)
                        {
                            horSpawn.Add(carSpawn);
                        }
                    }
                    if (horSpawn.Count > 0)
                    {
                        CarSpawnScript tempH = horSpawn[Random.Range(0, horSpawn.Count)];
                        tempH.SpawnCar();
                    }
                }
            }
        }
    }

    private void Update()
    {
        if(gameRunning)
        {
            timeCounter += Time.deltaTime;

            if(timeCounter >= (isInit? .2f : spawnDelay))
            {
                UpdateFuncToCall(isInit);
                isInit = false;
                timeCounter = 0;
            }
        }


        if(Input.GetMouseButtonDown(0))
        {
            // Click Crosses
            try
            {
                Vector3 mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                List<RaycastHit2D> hitResults = new List<RaycastHit2D>();
                Physics2D.Raycast(mousePos2D, Vector2.zero, filter, hitResults);

                if (hitResults.Count > 0)
                {
                    foreach (var hitRes in hitResults)
                    {
                        if (hitRes.collider != null)
                        {
                            if (hitRes.collider.CompareTag("CrossSimple"))
                            {
                                Crosses clickedCross = hitRes.collider.GetComponentInParent<Crosses>();
                                clickedCross.CrossClicked();
                            }
                        }
                    }
                }
            }
            catch
            {
                Debug.LogError("Click On Cross failed! Control.cs : 386; ");
            }
        }
    }
}
