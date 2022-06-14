using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_StartGameSimple : MonoBehaviour
{
    void Start()
    {
        Control.instance.StartGame("Debug",false);
    }
}
