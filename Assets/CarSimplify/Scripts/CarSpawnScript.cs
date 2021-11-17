using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSpawnScript : SimplifyParent
{
    public bool HorOrVert = true;
    [Space]
    [SerializeField] Crosses[] crossRefs;
    [SerializeField] GameObject carPrefab;




    public override void SimpleUpdate()
    {
        base.SimpleUpdate();

        SpawnCar();
    }

    public void SpawnCar()
    {
        GameObject inst = Instantiate(carPrefab, transform.position, transform.rotation);
        inst.GetComponent<CarSimple>().ManualInit(crossRefs, HorOrVert);
    }
}
