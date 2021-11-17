using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    [Header("Adjust")]
    public float spawnDelay= 1f;
    public float gridSize = 1.5f;

    [SerializeField] int SpawnCarAmountPerTurn = 2;




    [Header("Gameplay not change")]
    public int actualStep = 0;
    public static Control con;

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
        StartGame();
    }

    public void StartGame ()
    {
        crossesRefs = FindObjectsOfType<Crosses>();
        carSpawnRefs = FindObjectsOfType<CarSpawnScript>();

        StartCoroutine(UpdateFunc());
    }

    IEnumerator UpdateFunc()
    {
        yield return new WaitForSeconds(spawnDelay);

        UpdateElems();

        StartCoroutine(UpdateFunc());
    }

    void UpdateElems ()
    {
        foreach (var cs in carSpawnRefs)
        {
            cs.UpdateCheckIfSpawnAllowed();
        }
        foreach (var cs in carSpawnRefs)
        {
            cs.UpdateCorrespondingCheck();
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
        List<CarSpawnScript> possibleSpawnPos = new List<CarSpawnScript>();
        foreach (var carSpawn in carSpawnRefs)
        {
            if(carSpawn.AllowSpawnCar)
            {
                possibleSpawnPos.Add(carSpawn);
            }
        }

        for (int i = 0; i < SpawnCarAmountPerTurn; i++)
        {
            CarSpawnScript temp = possibleSpawnPos[Random.Range(0, possibleSpawnPos.Count)];
            temp.SpawnCar();
            possibleSpawnPos.Remove(temp);
        }
    }
}
