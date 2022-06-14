using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnScript : SimplifyParent
{
    public bool HorOrVert = true;
    public Crosses[] crossingCrosses;
    [Space]
    [SerializeField] Crosses[] crossRefs;
    [SerializeField] GameObject carPrefab;
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
        if (AllowSpawnCar)
        {
            GameObject inst = Instantiate(carPrefab, transform.position, transform.rotation);
            inst.GetComponent<CarSimple>().ManualInit(crossRefs, HorOrVert);

            if (Control.instance.actualSaveClass != null)
            {
                Control.instance.actualSaveClass.carsTotal++;
            }
        }
    }

    public bool CheckIfSpawnAllowed ()
    {
        foreach (var cros in crossingCrosses)
        {
            int tempIndexSelf = 0;
            int counter = -1;
            foreach (CarSpawnScript css in cros.carSpawnLock)       //Set selfIndex
            {
                counter++;
                if (css == this)
                {
                    tempIndexSelf = counter;
                }
            }

            foreach (Vector2 turn in cros.crossedInTurns)
            {
                if (turn.x == cros.TurnToLockSpwanPairs[tempIndexSelf])
                {
                    return false;
                }
            }
        }
        return true;
    }
}
