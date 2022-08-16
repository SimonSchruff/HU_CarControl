using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDataSave : MonoBehaviour
{
    private void Start()
    {
        if (!SQLSaveManager.instance)
            return;
        print("Data saving automatically...");
        SQLSaveManager.instance.StartPostCoroutine();

    }
}
