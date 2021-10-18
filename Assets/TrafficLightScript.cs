using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrafficLightScript : SimulatedParent
{
    public lightState state = lightState.green;
    [SerializeField] SpriteRenderer lightSprite;
    [SerializeField] Rigidbody2D clickableSprite;
    [SerializeField] Color[] redOrangeGreen;

    public int trafficLightID;
    public float clickedTime;
    float waitTime;

    public lightState stateAfterOrange;

    [SerializeField] Text text;

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
    }
    public void ChangeText (string changeTo)
    {
        try
        {
            text.text = changeTo;
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

    //    if (Assistance.assi.actualAssistance != Assistance.AssistanceTypes.Manual)
    //    {
    //        SimulationControlScript.sim.StartSim();
    //    }
    }
}
