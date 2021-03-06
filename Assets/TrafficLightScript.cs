using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightScript : SimulatedParent
{
    public List<string> DebugListEntries = new List<string>();

    [SerializeField] int simScore;

    public lightState state = lightState.green;
    [SerializeField] SpriteRenderer lightSprite;
    [SerializeField] Rigidbody2D clickableSprite;
    [SerializeField] Color[] redOrangeGreen;

    public TrafficLightScript correspondingTL;
    public int correspondingTLID;

    public int trafficLightID;
    public float clickedTime;
    float waitTime;

    public lightState stateAfterOrange;

    [SerializeField] Text text;
    [SerializeField] Text text2;

    public int waitingCarsCounter;

    public enum lightState
    {
        red,
        orange,
        green
    }

    private void Awake()
    {
        UpdateTrafficLight(false, lightState.green);
        ChangeText("");
        ChangeText("",false);

        if(correspondingTL == null)
        {
            Debug.LogError("Assign corresponding Traffic Light!", gameObject);
        }
    }

    private void Start()
    {
        if(simState == simulationState.game)
        {
            correspondingTLID = correspondingTL.trafficLightID;
        }
    }

    public int GetSimScore ()
    {
        return simScore;
    }


    public void ChangeText (string changeTo, bool changeTextOne = true)
    {
        try
        {
            (changeTextOne?text:text2).text = changeTo;
        }
        catch {}
    }

    void UpdateTrafficLight(bool setState = true, lightState newState = lightState.green) //UpdateTrafficLightsState
    {
        if (setState)
        {
            state = newState;
        }
        if(newState == lightState.green)
        {
            if(simState == simulationState.game)
            {
                waitingCarsCounter = 0;
            }
        }

        lightSprite.color = GetColorFromState();
    }

    public override void InitSimulation()       //What to do when Simulation starts
    {
        base.InitSimulation();

        simScore = 0;
        DebugListEntries.Clear();

        if(state == lightState.orange)
        {
            waitTime = GameManager.GM.timeCounter - clickedTime;
        }

        int correspondingTLID = correspondingTL.trafficLightID;
        foreach(TrafficLightScript tl in SimulationControlScript.sim.simTrafficLights)
        {
            if(correspondingTLID == tl.trafficLightID)
            {
                correspondingTL = tl;
            }
        }
    }
    public override void UpdateSimulation(float simStep)
    {
        base.UpdateSimulation(simStep);

        if(state == lightState.orange)
        {
            waitTime -= simStep;
            if (waitTime <= 0)
            {
                UpdateTrafficLight(true, stateAfterOrange);
            }
        }
    }

    void SetOrangePhase (lightState newState)
    {
        if (state != lightState.orange)
        {
            stateAfterOrange = newState;
            state = lightState.orange;
            lightSprite.color = GetColorFromState();
            clickedTime = simState == simulationState.game ? GameManager.GM.timeCounter : SimulationControlScript.sim.simTimeCounter;

            if(simState == simulationState.game)
                StartCoroutine(setOrangeDelay());
            else
            {
                waitTime = 1f;
            }
        } 
    }
    IEnumerator setOrangeDelay ()
    {
        yield return new WaitForSecondsRealtime(1f);      //Set up delay after Orange
        UpdateTrafficLight(true, stateAfterOrange);
    }
    Color GetColorFromState()
    {
        return redOrangeGreen[(int)state];
    }

    public void LightClicked ()
    {
        if (state == lightState.green)
        {
            SetOrangePhase(lightState.red);
        }
        else
        {
            SetOrangePhase(lightState.green);
        }
    }

    public void AddDebubInfo (string typ, int amount, GameObject senderCar)
    {
        string temp = typ + "_" + amount + "_" + (senderCar == null ? "" : senderCar.name);
        DebugListEntries.Add(temp);
    }

    public void AddScoreToTL (int amount, string info)
    {
        simScore += amount;

        string temp = info + "_" + amount;      //DebugInfo
        DebugListEntries.Add(temp);
    }
}
