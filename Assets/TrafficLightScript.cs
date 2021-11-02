using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightScript : SimulatedParent
{
    [SerializeField] List<string> DebugListEntries = new List<string>();

    public lightState state = lightState.green;
    [SerializeField] SpriteRenderer lightSprite;
    [SerializeField] Rigidbody2D clickableSprite;
    [SerializeField] Color[] redOrangeGreen;

    public int trafficLightID;
    public float clickedTime;
    float waitTime;

    public lightState stateAfterOrange;

    [SerializeField] Text text;
    [SerializeField] Text text2;

    Camera camRef;
    public enum lightState
    {
        red,
        orange,
        green
    }

    private void Awake()
    {
        UpdateTrafficLight(false, lightState.green);
        camRef = Camera.main;
        ChangeText("");
        ChangeText("",false);
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
        lightSprite.color = GetColorFromState();
    }

    public override void InitSimulation()       //What to do when Simulation starts
    {
        base.InitSimulation();

        if(state == lightState.orange)
        {
            waitTime = GameManager.GM.timeCounter - clickedTime;
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

    public void AddEntryToDebugListing (Score.PointTypes typ, int amount, GameObject senderCar)
    {
        string temp = typ.ToString() + "_" + amount + "_" + (senderCar == null ? "" : senderCar.name);
        DebugListEntries.Add(temp);
    }
}
