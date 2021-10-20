using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class NBackGameManager : MonoBehaviour
{
    public static NBackGameManager Instance; 

    
    [HideInInspector]
    public enum GameState
    {
        timer,
        instructions, 
        running, 
        gameOver
    }
    public GameState gameState; 

    [HideInInspector]
    public enum CurrentLevel
    {
        training,
        level01,
        level02,
        level03
    }
    public CurrentLevel currentLevel;

    
    
    
    [Header("Timer")]
    public float letterShowSec = 0.5f; 
    public float letterWaitSec = 2.5f; 
    float timer; 
    float hideTimer; 


    [Header("Letter Objects, Lists, and Variables")]
    public int n = 2; 
    public string currentLetter; 
    //Letters that are randomly selected are directly assigned to List in inspector
    public List<string> lettersOfChoice = new List<string>(); 
    public List<string> usedLetters = new List<string>(); 
    string correctLetter = "A";
    public bool inputHappened; 
    bool newLetterRdy = true; 
    bool newTimerRdy = true; 



    [Header("UI References")]
    public GameObject InstructionObjects; 
    public GameObject RunningGameObjects; 
    public GameObject TimerGameObjects; 
    public Text levelTextObj; 
    public Text timerTextObj; 
    public GameObject letterObject; 
    public Text letterTextObj; 
    public Button[] Buttons = new Button[2]; 

    
    /*
        [] Create Timer before game starts
        [] Functionality of levels / trial level
        [] Update Instructions UI 
        [] Visually Disable Buttons for first n Numbers
    */

    void Awake()
    {
        //Singleton
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
    
        gameState = GameState.instructions; 

        
        //letterObject = GameObject.FindGameObjectWithTag("letterObj"); 
        //letterTextObj = letterObject.GetComponentInChildren<Text>();

        //levelTextObj = GameObject.FindGameObjectWithTag("levelTextObj").GetComponent<Text>();
        //timerTextObj = GameObject.FindGameObjectWithTag("timerTextObj").GetComponent<Text>();

    }

    void Update()
    {
        

        switch(gameState)
        {
            case GameState.timer: 
                TimerGameObjects.SetActive(true); 
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(false);
                
                if(newTimerRdy)
                    StartCoroutine(Timer()); 
            break; 

            case GameState.instructions: 
                TimerGameObjects.SetActive(false);
                InstructionObjects.SetActive(true); 
                RunningGameObjects.SetActive(false); 
            break; 

            case GameState.running: 
                TimerGameObjects.SetActive(false);
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(true); 

                //Start Game Logic
                if(newLetterRdy)
                    StartCoroutine(OneLetterPeriod()); 
            break; 

            case GameState.gameOver: 
            //Show Restart Button ? 
            //Save Score etc. 
            break; 
        }


        
        
    }

    IEnumerator OneLetterPeriod()
    {
        newLetterRdy = false; 

        // If very first letter "correct" usedLetters.count - 2 is out of Range
        // Disable Buttons if (Match/Mismatch is not possible)
            if( usedLetters.Count >= n )
            {
                correctLetter = usedLetters[usedLetters.Count - n]; 
                foreach(Button b in Buttons)
                    b.interactable = true;
                
            }
            else
            {
                foreach(Button b in Buttons)
                    b.interactable = false;
            }
        
        

        letterTextObj.text = RandomizeLetter(); 
        currentLetter = letterTextObj.text; 
        usedLetters.Add(currentLetter); 

        inputHappened = false; 
        letterObject.SetActive(true); 

        yield return new WaitForSeconds(letterShowSec); 

        letterObject.SetActive(false); 

        yield return new WaitForSeconds(letterWaitSec); 

        //Events if no Input happened during one letter period
        if(!inputHappened && currentLetter == correctLetter)
            Debug.Log("Match missed! "); 

        if( usedLetters.Count > n )
        {
            if(!inputHappened && currentLetter != correctLetter)
            Debug.Log("Mismatch missed!");
        }
         


        newLetterRdy = true; 


    }


    
    IEnumerator Timer()
    {
        newTimerRdy = false; 

        // Set to three
        timerTextObj.text = "3"; 
        yield return new WaitForSeconds(1); 
        timerTextObj.text = "2";    
        yield return new WaitForSeconds(1); 
        timerTextObj.text = "1"; 
        yield return new WaitForSeconds(1); 

        gameState = GameState.running; 
    }
    

    string RandomizeLetter()
    {
        string newLetter = "A"; 
     
        // Create 1/3 chance of "correct" letter 
        int rand = Random.Range(1,4); 
        if(usedLetters.Count <= n )
            rand = 1; 

        switch(rand)
        {
            case 1: 
            case 2: 
                //Create new random letter
                string tempLetter = "A"; 
                tempLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count)]; 
                // If random letter is the same as correct letter, repeat randomizing until different letter
                //Does this affect randomness of letters ? 
                while( tempLetter  == correctLetter )
                {
                    tempLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count)]; 
                }
                newLetter = tempLetter; 
            break; 
            //Show "Correct" Letter
            case 3: 
                
                Debug.Log("Correct Letter"); 
                newLetter = correctLetter; 
            break; 

        }

        return newLetter; 

        
    }

    public void ChangeGameState(int i)
    {
        //Assign int in button etc. 
        switch(i)
        {
            case 0 : 
            newTimerRdy = true; 
            gameState = GameState.timer; 
            break; 
            case 1 : 
            gameState = GameState.instructions; 
            break; 
            case 2 : 
            gameState = GameState.running; 
            break; 
        }
    }

    public void ChangeLevels(int i)
    {
        switch(i)
        {
            case 0 : 
            currentLevel = CurrentLevel.training; 
            break; 
            case 1 : 
            currentLevel = CurrentLevel.level01; 
            break; 
            case 2 : 
            currentLevel = CurrentLevel.level02; 
            break; 
            case 3 : 
            currentLevel = CurrentLevel.level03; 
            break; 
        }
    }

    public void MatchMismatchButtonEvent ( int i )
    {
        if( inputHappened )
            return; 
        else
            inputHappened = true; 

        switch(i)
        {
            //Match Button 
            case 0: 
                if ( currentLetter == correctLetter)
                {
                    Debug.Log("Correct Answer"); 
                }
                else
                {
                    Debug.Log("False Answer"); 
                }

            break; 
            //Mismatch Button 
            case 1: 
                if ( currentLetter == correctLetter)
                    {
                        Debug.Log("False Answer"); 
                    }
                    else
                    {
                        Debug.Log("Correct Answer"); 
                    }

            break; 
        }
    }

    
}
