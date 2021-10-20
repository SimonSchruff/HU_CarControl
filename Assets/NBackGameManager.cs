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
        levelFinished, 
        gameOver
    }
    public GameState gameState; 


    
    
    
    [Header("Timer")]
    public float letterShowSec = 0.5f; 
    public float letterWaitSec = 2.5f; 
    float timer; 
    float hideTimer; 


    [Header("Letter Objects, Lists, and Variables")]
    public int currentLevel = 0; 
    public int n = 2; 
    public string currentLetter; 
    //Letters that are randomly selected are directly assigned to List in inspector
    public List<string> lettersOfChoice = new List<string>(); 
    public List<string> usedLetters = new List<string>(); 
    string correctLetter = "A";
    public bool inputHappened; 
    int stimuli = 0; 
    public int stimuliShown = 25; 
    int levelInt = 0; 
    
    bool newLetterRdy = true; 
    bool newTimerRdy = true; 
    bool switchedLevels = false; 



    [Header("UI References")]
    public GameObject InstructionObjects; 
    public GameObject RunningGameObjects; 
    public GameObject TimerGameObjects; 
    public GameObject LevelFinishedGameObjects; 
    public GameObject GameOverObjects; 
    public Text levelTextObj; 
    public Text timerTextObj; 
    public GameObject letterObject; 
    public Text letterTextObj; 
    public Button[] Buttons = new Button[2]; 

    
    /*
        [x] Create Timer before game starts
        [x] Functionality of levels / trial level
        [] Update Instructions UI 
        [x] Visually Disable Buttons for first n Numbers
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

        gameState = GameState.instructions; 
    }

    void Update()
    {
        

        switch(gameState)
        {
            case GameState.timer: 
                TimerGameObjects.SetActive(true); 
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(false);
                LevelFinishedGameObjects.SetActive(false); 
                GameOverObjects.SetActive(false); 
                
                if(newTimerRdy)
                    StartCoroutine(Timer()); 
            break; 

            case GameState.instructions: 
                TimerGameObjects.SetActive(false);
                InstructionObjects.SetActive(true); 
                RunningGameObjects.SetActive(false); 
                LevelFinishedGameObjects.SetActive(false); 
                GameOverObjects.SetActive(false); 
            break; 

            case GameState.running: 
                TimerGameObjects.SetActive(false);
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(true);
                LevelFinishedGameObjects.SetActive(false);
                GameOverObjects.SetActive(false);   

                if(stimuli > stimuliShown)
                {
                    switchedLevels = false; 
                    currentLevel++; 
                }

                //One Letter shown equals one run of coroutine
                if(newLetterRdy)
                    StartCoroutine(OneLetterPeriod()); 
            break; 

            case GameState.levelFinished:
                TimerGameObjects.SetActive(false); 
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(false);
                LevelFinishedGameObjects.SetActive(true); 
                GameOverObjects.SetActive(false); 

            break; 

            case GameState.gameOver: 
                TimerGameObjects.SetActive(false); 
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(false);
                LevelFinishedGameObjects.SetActive(false); 
                GameOverObjects.SetActive(true); 
            break; 
        }

        //After 25 Stimuli is reached level will be changed -> reset stimuli, change gameState etc. 
        if(!switchedLevels)
        {
            switch(currentLevel)
            {
                case 0: //Training Level

                break; 
                case 1: 
                levelTextObj.text = "Level 01"; 
                stimuli = 0; 
                gameState = GameState.levelFinished; 
                switchedLevels = true; 
                usedLetters.Clear(); 
                break; 
                case 2: 
                levelTextObj.text = "Level 02"; 
                stimuli = 0; 
                gameState = GameState.levelFinished; 
                switchedLevels = true; 
                usedLetters.Clear(); 
                break;  
                case 3: 
                levelTextObj.text = "Level 03"; 
                stimuli = 0; 
                gameState = GameState.gameOver; 
                switchedLevels = true; 
                
                break; 
            }
        }


        // Disable Button if input happened, or at beginning when no correct answer is possible
        if( inputHappened || usedLetters.Count < n + 1)
        {
            foreach(Button b in Buttons)
                b.interactable = false;
        }
        else if ( !inputHappened )
        {
            foreach(Button b in Buttons)
                b.interactable = true;
        }
        
        
    }

    IEnumerator OneLetterPeriod()
    {
        newLetterRdy = false;
        inputHappened = false;  
     
        // Disable Buttons if (Match/Mismatch is not possible) 
        // Correct letter only possible if usedLetters.Count is >= then n 
        if( usedLetters.Count >= n )
        {
            correctLetter = usedLetters[usedLetters.Count - n]; 
        }
        
        //Get new random letter, add it to list and show it
        letterTextObj.text = RandomizeLetter(); 
        currentLetter = letterTextObj.text; 
        usedLetters.Add(currentLetter); 

        letterObject.SetActive(true); 

        yield return new WaitForSeconds(letterShowSec); 

        letterObject.SetActive(false); 

        yield return new WaitForSeconds(letterWaitSec); 

        // If no Input happened during full letter period
        if(!inputHappened && currentLetter == correctLetter)
            Debug.Log("Match missed! "); 

        if( usedLetters.Count > n )
        {
            if(!inputHappened && currentLetter != correctLetter)
            Debug.Log("Mismatch missed!");
        }
         

        stimuli++; 
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
        newTimerRdy = true;  
    }
    

    string RandomizeLetter()
    {
        string newLetter = "X"; 
     
        // Create 1/3 chance of "correct" letter 
        // For first n letters no "correct" letter is possible
        int rand = Random.Range(1,4); 
        if(usedLetters.Count <= n )
            rand = 1; 

        switch(rand)
        {
            case 1: 
            case 2: 
                string tempLetter = "X"; 
                tempLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count)]; 
                // If random letter is the same as correct letter, repeat randomizing until different letter
                //Does this affect randomness of letters ? 
                while( tempLetter  == correctLetter )
                {
                    tempLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count)]; 
                }
                newLetter = tempLetter; 
            break; 
            case 3: 
                //Debug.Log("Correct Letter"); 
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
                gameState = GameState.timer; 
            break; 
            case 1 : 
                gameState = GameState.instructions; 
            break; 
            case 2 : 
                gameState = GameState.running; 
            break; 
            case 3: 
                gameState = GameState.levelFinished; 
            break; 
            case 4: 
                gameState = GameState.gameOver; 
            break; 
        }
    }

    public void MatchMismatchButtonEvent ( int i )
    {
        // No double clicking of button during one full letter period
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
