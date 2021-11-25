using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    [SerializeField] int trialDurationInMin = 5;
    [Header("Adjust")]
    public float spawnDelay= 1f;
    public float gridSize = 1.5f;

    [SerializeField] int SpawnCarAmountPerTurn = 2;

    [SerializeField] Camera camRef;
    [SerializeField] ContactFilter2D filter;
    [SerializeField] LayerMask mask;
        


    [Header("Gameplay not change")]
    public int actualStep = 0;
    public static Control con;

    public SaveTrialClass actualSaveClass;
    public List<SaveTrialClass> allSaveClasses = new List<SaveTrialClass>();

    public Crosses[] crossesRefs;
    public CarSpawnScript[] carSpawnRefs;

    private void Awake()
    {
        if (con == null)
        {
            con = this;
        }
        else
        {
            Destroy(gameObject);
        }

        StartCoroutine(StartGameDelay());
    }

    IEnumerator StartGameDelay()
    {
        yield return new WaitForSecondsRealtime(2);
        StartGame("Base");
    }

    public void StartGame (string TrialName)
    {
        crossesRefs = FindObjectsOfType<Crosses>();
        carSpawnRefs = FindObjectsOfType<CarSpawnScript>();

        StartCoroutine(UpdateFunc());
        StartCoroutine(FinishTrial());

        actualSaveClass = gameObject.AddComponent<SaveTrialClass>();
        actualSaveClass.trialName = TrialName;
        actualSaveClass.assistance = AssistanceSelectScript.assiSel.actualAssiSelect.ToString();
        allSaveClasses.Add(actualSaveClass);

        if (AssistanceSelectScript.assiSel.actualAssiSelect == AssistanceSelectScript.AssiSelectStates.Area)
        {
            actualSaveClass.percentageArea = 1;
        }
        else if (AssistanceSelectScript.assiSel.actualAssiSelect == AssistanceSelectScript.AssiSelectStates.Specific)
        {
            actualSaveClass.percentageSpecific = 1;
        }
        else if (AssistanceSelectScript.assiSel.actualAssiSelect == AssistanceSelectScript.AssiSelectStates.Select)
        {
            actualSaveClass.InitChangeAssistanceActive(SimplAssis.assi.actualAssistance == SimplAssis.assiState.areaHelp);
            
        }
    }
    IEnumerator FinishTrial ()
    {
        yield return new WaitForSeconds(trialDurationInMin * 60);
        FinishTrialFunc();
    }

    void FinishTrialFunc ()
    {
        actualSaveClass.FinishTrial();

        actualSaveClass.score = ScoreSimple.sco.GetScore(); // Set score
        ScoreSimple.sco.ResetScore();   //Reset score

        foreach (var cars in FindObjectsOfType<CarSimple>())
        {
            Destroy(cars.gameObject);
        }
        SimplAssis.assi.UpdateAssistance();

        StopCoroutine(UpdateFunc());

        actualSaveClass = null;
    }

    IEnumerator UpdateFunc()
    {
        yield return new WaitForSeconds(spawnDelay);

        UpdateElems();
        SimplAssis.assi.UpdateAssistance();

        StartCoroutine(UpdateFunc());
    }

    void UpdateElems ()
    {
        foreach (var cs in carSpawnRefs)
        {
            cs.UpdateCheckIfSpawnAllowed();
        }

        GetPossibleSpawnerAndSpawnCar();

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
    public void GetPossibleSpawnerAndSpawnCar ()
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


            if (Random.Range(0, 2) == 1)
            {
                foreach (var carSpawn in carSpawnRefs)
                {
                    if (carSpawn.AllowSpawnCar && !carSpawn.HorOrVert)
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
                    if (carSpawn.AllowSpawnCar && carSpawn.HorOrVert)
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
                    if (carSpawn.AllowSpawnCar && carSpawn.HorOrVert)
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
                    if (carSpawn.AllowSpawnCar && !carSpawn.HorOrVert)
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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            try         // Click Crosses
            {
                Vector3 mousePos = camRef.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

                List <RaycastHit2D> hitResults = new List<RaycastHit2D>();
                Physics2D.Raycast(mousePos2D, Vector2.zero, filter, hitResults);

                if(hitResults.Count > 0)
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

                //RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero, mask);
                //if (hit.collider != null)
                //{
                //    if (hit.collider.CompareTag("CrossSimple"))
                //    {
                //        Crosses clickedCross = hit.collider.GetComponentInParent<Crosses>();
                //        clickedCross.CrossClicked();
                //    }
                //}
            }
            catch { }
        }
    }
}
