using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_StartGameSimple : MonoBehaviour
{
    public TrialStartLogic StartTrial;
    
    
    void Start()
    {
        if (!Control.instance) {
            print("Cannot find Control.cs in Debug_StartGame!");
            return;
        }
        
        // No start trials specified -> Start debug trial
        if (!StartTrial) {
          print("Start debug trial, because no trial was specified in debug_startGame!");
          Control.instance.StartGame("Debug",false);
          return;
        }
        
        StartTrial.ActivateSession();
    }
}
