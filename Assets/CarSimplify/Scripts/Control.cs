using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    [Header("Adjust")]
    public float speed = 1f;
    public float spawnDelay= 1f;




    [Header("Gameplay not change")]
    public int actualStep = 0;
    public float gridSize = 1.5f;
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
        yield return new WaitForSecondsRealtime(spawnDelay);

        UpdateElems();

        StartCoroutine(UpdateFunc());
    }

    void UpdateElems ()
    {
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
    public  CarSpawnScript GetPossibleSpawnerAndSpawnCar ()
    {




        return null;
    }
}
