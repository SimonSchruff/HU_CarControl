using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Header ("Adjust points for success")]
    public int carFinished = 5, emergencyFinished = 7;
    [Header("Adjust points for failture")]
    public int carCrash = 2, emergencyCrash = 5, carInWater = 8, emergencyInWater = 10, emergencyWait = 1;
    [Space]
    [SerializeField] int bigHighlightLimit = 5;
    [Space]
    [SerializeField] AnimationClip goodFeedback, badFeedback;
    static public Score sc;
    int score = 0;
    Text scoreText;
    Animator animator;

    [SerializeField] Text debugTextRef;

    SimulationControlScript simControl;

    //Counters
    int couCarFinished,
        couEmergencyFinished,
        couCarCrash,
        couEmergencyCrash,
        couCarInWater,
        couEmergencyInWater,
        couEmergencyWait;

    public enum PointTypes
    {
        carFinished,
        emergencyFinished,
        carCrash,
        emergencyCrash,
        carInWater,
        emergencyInWater,
        emergencyWait,
        normalCarWait
    }

    private void Awake()
    {
        if (sc == null)
            sc = this;
        else
            Destroy(this);

        scoreText = GetComponentInChildren<Text>();
        animator = GetComponentInChildren<Animator>();
        scoreText.text = "0";
    }
    private void Start()
    {
        simControl = SimulationControlScript.sim;
    }

    public void SetDebugText (string textToSet)
    {
        debugTextRef.text = textToSet;
    }
     
    public int GetScore ()
    {
        return score;
    }

    public int AddPoints (PointTypes pointReason, SimulatedParent.simulationState simState)
    {
        int amount = 0;
        switch (pointReason)        // Assign Point values to types
        {
            case PointTypes.carFinished:
                amount = carFinished;
                break;
            case PointTypes.emergencyFinished:
                amount = emergencyFinished;
                break;
            case PointTypes.carCrash:
                amount = -carCrash;
                break;
            case PointTypes.emergencyCrash:
                amount = -emergencyCrash;
                break;
            case PointTypes.carInWater:
                amount = -carInWater;
                break;
            case PointTypes.emergencyInWater:
                amount = -emergencyInWater;
                break;
            case PointTypes.emergencyWait:
                amount = -emergencyWait;
                break;
        }

        if(simState == SimulatedParent.simulationState.game)
        {
            if (amount == 0)
                return score;
            score += amount;
            scoreText.text = score.ToString();

            if(Mathf.Abs(amount) >= bigHighlightLimit)
            {
                if (amount > 0)
                    animator.SetTrigger("goodEX");
                else
                    animator.SetTrigger("badEX");
            }        
            else
            {
                if (amount > 0)
                    animator.SetTrigger("good");
                else
                    animator.SetTrigger("bad");
            }

            return score;
        }
        else    // point control in simulation
        {
            simControl.simScore += amount;
            return score;
        }
    }
}
