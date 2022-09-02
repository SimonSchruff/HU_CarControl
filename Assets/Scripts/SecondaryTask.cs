using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class SecondaryTask : MonoBehaviour
{
    

    [Header("Key Bindings")]
    public KeyCode resetKey = KeyCode.Space; 


    [Header("Object References")]
    public GameObject indicator;

    [Header("Training Session")]
    public bool isTrainingSession;
    public float trainingTime; 
    public Text trainingText;
    //public GameObject continueButton;
    private float _trainingTimer; 


    [Header("Variables")]
    public float travelDistance = 37.5f; 
    public float moveSpeed; 
    public float timeRange = 60f; 
    public float timeToGiveInput = 10f; 
    int direction;
    int rounds = 0;
    int score = 0; 


    #region Position Vectors
    Vector3 startPos; 
    Vector3 currentStartPos; 
    Vector3 startPosTop; 
    Vector3 startPosBot; 
    Vector3 currentPos; 
    #endregion

    #region Timer Variables
    bool reset = false; 
    bool timerActive = true; 
    bool error = false; 
    float timer;
    bool cooldown; 
    float cooldownTimer; 
    float inputTimer = 10f; 
    #endregion
    
    #region Game State
    public enum CurrentState
    {
        cooldown, 
        baseState, 
        errorStateTop, 
        errorStateBot
    }
    [Header("Current Indicator State")]
    public CurrentState currentState; 
    #endregion


    void Start()
    {
        //Get Local Starting Positions
        startPos = indicator.transform.localPosition;
        startPosTop = new Vector3(startPos.x , startPos.y + travelDistance ,0); 
        startPosBot = new Vector3(startPos.x , startPos.y - travelDistance ,0); 

        currentStartPos = startPos; 

        //What direction does indicator travel initally
        int temp = 0; 
        temp = Random.Range(1,3); 
        switch(temp)
        {
            case 1: 
                direction = 1; 
                break; 
            case 2: 
                direction = -1; 
                break; 
        }



        //Timer; For first time reset is only possible after 10sec to give player time to settle
        timer = Random.Range(5, timeRange); 
        cooldownTimer = timeRange - timer; 
        //print("Timer: " + timer + "; Cooldown: " + cooldownTimer); 

        currentState = CurrentState.baseState; 


        
    }

    void Update()
    {
        GetCurrentPos(); 

        Timer(); 

        if(Input.GetKeyDown(resetKey))
        {

            if(currentState == CurrentState.baseState || currentState == CurrentState.cooldown)
            {
                if(!isTrainingSession)
                {
                    ScoreSimple.sco.UpdateScore(-1);  // False Alarm
                    Control.instance.actualSaveClass.sec_FalseAlarm++; 
                }
                else
                {
                    score--;
                    StartCoroutine(ShowTrainingText("Wrong Input! -1 Score Deduction"));
                }
            }
            else if(currentState == CurrentState.errorStateTop || currentState == CurrentState.errorStateBot)
            {
                if(!isTrainingSession)
                {
                    Control.instance.actualSaveClass.sec_Correct++;
                    Control.instance.actualSaveClass.sec_TotalAlarms++;
                }
                else
                    StartCoroutine(ShowTrainingText("Correct Input!"));

                rounds++; 
                inputTimer = timeToGiveInput; 
                currentState = CurrentState.cooldown;    
            }

            indicator.transform.localPosition = startPos;
        }

        switch(currentState)
        {
            case CurrentState.baseState : 
                currentStartPos = startPos; 
                error = false; 
                timerActive = true; 
                cooldown = false; 
                break; 

            case CurrentState.errorStateTop : 
                currentStartPos = startPosTop; 
                timerActive = false; 
                error = true; 
                cooldown = false; 
                break; 

            case CurrentState.errorStateBot : 
                currentStartPos = startPosBot; 
                timerActive = false; 
                error = true; 
                cooldown = false; 
                break; 

            case CurrentState.cooldown: 
                currentStartPos = startPos; 
                error = false; 
                timerActive = false; 
                cooldown = true; 
                break; 
        }

        if(isTrainingSession)
        {
            _trainingTimer += Time.deltaTime;
            if(_trainingTimer > trainingTime)
                FragebogenManager.instance.NextQuestion();        
        }


        Move(currentStartPos); 

    }

    void Timer()
    {
        

        //TIMER DURING BASE STATE
        if(timerActive && !cooldown)
            timer -= Time.deltaTime; 

        if(timer <= 0)
        {
            //Reset to new pos
            reset = true; 
            timerActive = false; 
            cooldown = true;
            timer = Random.Range(1, timeRange - timeToGiveInput); 
            cooldownTimer = timeRange - timer;
            print("Timer: " + timer + "; Cooldown: " + cooldownTimer); 
            
        }

        //COOLDOWN TIMER
        if(cooldown)
            cooldownTimer -= Time.deltaTime; 

        if(cooldownTimer <= 0 && cooldown)
        {
            currentState = CurrentState.baseState; 
        }

        // TIMER DURING ERROR STATES
        if(error)
            inputTimer -= Time.deltaTime; 

        if(inputTimer <= 0 )
        {
            // Player did not give input 
            currentState = CurrentState.cooldown; 
            inputTimer = timeToGiveInput;
            if (!isTrainingSession)
            {
                try
                {
                    ScoreSimple.sco.UpdateScore(-2); //Missed
                    Control.instance.actualSaveClass.sec_Misses++;
                    Control.instance.actualSaveClass.sec_TotalAlarms++;
                    rounds++; 

                }
                catch
                {
                    Debug.LogError("Adding Missed Ref Exception!"); 
                }
            }
            else
            {
                StartCoroutine(ShowTrainingText("You did not give an input! -2 Score Deduction")); 
                score -= 2;
                rounds++; 
            }
                
        }
    }

    void Move(Vector3 basePos)
    {
        if( currentPos.y > basePos.y + travelDistance)
        {
            //Reached Top
            if(reset)
            {
                currentState = CurrentState.errorStateTop ;
                reset = false; 
            }
            
            direction = -1; 
        }
        else if( currentPos.y < basePos.y - travelDistance)   
        {
            //Reached Bot
            if(reset)
            {
                currentState = CurrentState.errorStateBot ;
                reset = false; 
            }
            
            direction = 1; 
        }
           
        indicator.transform.position += new Vector3(0, moveSpeed * direction * Time.deltaTime ,0); 
    }


    Vector3 GetCurrentPos()
    {
        currentPos = indicator.transform.localPosition; 
        return currentPos; 
    }

    IEnumerator ShowTrainingText(string text)
    {
        trainingText.text = text;
        trainingText.gameObject.SetActive(true); 
        yield return new WaitForSeconds(2);
        trainingText.gameObject.SetActive(false);
    }



}
