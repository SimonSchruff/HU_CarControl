using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CarSpawnScript : SimplifyParent
{
    [FormerlySerializedAs("HorOrVert")] public bool IsHorizontal = true;
    public Crosses[] crossingCrosses;
    [Space]
    [SerializeField] Crosses[] crossRefs;
    [SerializeField] GameObject carPrefab;
    public GameObject[] CarPrefabs;
    public bool AllowSpawnCar = true;
    public CarSpawnScript correspondingSpawner;

    SpriteRenderer arrow;

    private void Start()
    {
        arrow = GetComponentInChildren<SpriteRenderer>();
    }

    public override void SimpleUpdate()
    {
        base.SimpleUpdate();
    }

    public void UpdateCheckIfSpawnAllowed ()
    {
        AllowSpawnCar = CheckIfSpawnAllowed();
        arrow.color = AllowSpawnCar ? Color.green : Color.red;
    }
    
    public void DisableAllowSpawn()
    {
        AllowSpawnCar = false;
        arrow.color = Color.red;
    }

    public void SpawnCar()
    {
        if (!AllowSpawnCar)
            return;
        
        // Check if Save Class is not null and save different car spawn amounts
        bool canSave = false;
        if (Control.instance.actualSaveClass != null)
        {
            canSave = true;
            Control.instance.actualSaveClass.carsTotal++;
        }
        
        // Spawn different vehicle types randomly 
        int i = Random.Range(1, CarPrefabs.Length + 1);
        switch (i)
        {
            case 1:
                GameObject car00Inst = Instantiate(CarPrefabs[0], transform.position, transform.rotation);
                car00Inst.GetComponent<CarSimple>().ManualInit(crossRefs, IsHorizontal);
                if(canSave)
                    Control.instance.actualSaveClass.cars00Total++;
                break;
            case 2:
                GameObject car01Inst = Instantiate(CarPrefabs[1], transform.position, transform.rotation);
                car01Inst.GetComponent<CarSimple>().ManualInit(crossRefs, IsHorizontal);
                if(canSave)
                    Control.instance.actualSaveClass.cars01Total++;
                break;
            case 3:
                GameObject car02Inst = Instantiate(CarPrefabs[2], transform.position, transform.rotation);
                car02Inst.GetComponent<CarSimple>().ManualInit(crossRefs, IsHorizontal);
                if(canSave)
                    Control.instance.actualSaveClass.cars02Total++;
                break;
                
            /*
           GameObject inst = Instantiate(carPrefab, transform.position, transform.rotation);
           inst.GetComponent<CarSimple>().ManualInit(crossRefs, HorOrVert);
           */
        }
    }

    public bool CheckIfSpawnAllowed ()
    {
        foreach (var cros in crossingCrosses)
        {
            int tempIndexSelf = 0;
          //  int counter = -1;

            for (int i = 0; i < cros.carSpawnLock.Length; i++)
            {
                if(cros.carSpawnLock[i] == this)
                {
                    tempIndexSelf = i;
                }
            }
            /*
            foreach (CarSpawnScript css in cros.carSpawnLock)       //Set selfIndex
            {
                counter++;
                if (css == this)
                {
                    tempIndexSelf = counter;
                }
            }
            */

            
            /*
            foreach (Vector2 turn in cros.crossedInTurns)
            {
                if (turn.x == cros.TurnToLockSpwanPairs[tempIndexSelf])
                {
                    return false;
                }
            }
            */
            
            foreach (var turn in cros.crossedInTurnsDictionary)
            {
                if (turn.Value.x == cros.TurnToLockSpwanPairs[tempIndexSelf])
                {
                    return false;
                }
            }
        }
        return true;
    }
}
