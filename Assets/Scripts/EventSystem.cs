using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventSystem : MonoBehaviour
{
    public static EventSystem es;
    private void Awake()
    {
        es = this;
    }

    public event Action onTrafficGreen;

    public void TrafficLightChangesToGreen (TrafficLightScript trafficLightRef)
    {

    }
}
