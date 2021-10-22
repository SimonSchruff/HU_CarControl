using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 


public class NBackGameManager : MonoBehaviour
{
    public static NBackGameManager Instance; 


    [System.Serializable]
    public struct LevelData
    {
        public int playerID; 
        public int level; 
        public float totalMatches;  
        public float totalMismatches;
        public float correctlyMatched; 
        public float falseAlarm; 
        public float correctlyMismatched; 
        public float falseAlarmMismatch;
        public float missedMatches; 
        public float missedMismatches; 
        
    }

    public LevelData levelData; 

    
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
    // PUBLIC VARIABLES
    public int n = 2;
    public int stimuliShown = 25;  
    
    // LISTS
    //Letters that are randomly selected are directly assigned to List in inspector
    public List<string> lettersOfChoice = new List<string>(); 
    public List<string> usedLetters = new List<string>(); 

    //  VARIABLES
    [HideInInspector]
    public int currentLevel = 0; 
    [HideInInspector]
    public string currentLetter; 
    string correctLetter = "A";
    int stimuli = 0; 
    int levelInt = 0; 

    //BOOLS
    [HideInInspector]
    public bool inputHappened; 
    bool newLetterRdy = true; 
    bool newTimerRdy = true; 
    bool switchedLevels = false; 

    // All possible score events
    [Header("Score")]
    public int correctMatch; 
    public int wrongMatch; 
    public int correctMismatch; 
    public int wrongMismatch;
    public int missedMatch; 
    public int missedMismatch; 
    public int totalCorrectLetter; 




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
    public Text[] scoreUINumbers = new Text[8]; 

    
    /*
        [x] Reset Level Fct()
        [x] Save Percentage scores for each level 
        [] Upload to 000webhost in own .php file
        [x] Clean UP Inspector Window 
        [x] Update Instructions UI 
        [x] Create Timer before game starts
        [x] Functionality of levels / trial level
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
        else
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

                if(stimuli >= stimuliShown)
                {
                    SaveScoreData(); 
                    switchedLevels = false; 
                    currentLevel++; 
                }

                //One Letter shown equals one run of coroutine
                if(newLetterRdy && stimuli < stimuliShown)
                    StartCoroutine(OneLetterPeriod()); 
            break; 

            case GameState.levelFinished:
                TimerGameObjects.SetActive(false); 
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(false);
                LevelFinishedGameObjects.SetActive(true); 
                //Set Score UI 
                {
                    //Match Numbers
                    scoreUINumbers[0].text = levelData.totalMatches.ToString(); 
                    scoreUINumbers[1].text = levelData.correctlyMatched.ToString(); 
                    scoreUINumbers[2].text = levelData.falseAlarm.ToString(); 
                    scoreUINumbers[3].text = levelData.missedMatches.ToString(); 
                    //Mismatch Numbers
                    scoreUINumbers[4].text = levelData.totalMismatches.ToString(); 
                    scoreUINumbers[5].text = levelData.correctlyMismatched.ToString(); 
                    scoreUINumbers[6].text = levelData.falseAlarmMismatch.ToString(); 
                    scoreUINumbers[7].text = levelData.missedMismatches.ToString(); 
                }
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
                    gameState = GameState.levelFinished; 
                    levelTextObj.text = "Level 01"; 
                    ResetForNewLevel(); 
                break; 
                case 2: 
                    levelTextObj.text = "Level 02"; 
                    gameState = GameState.levelFinished;
                    ResetForNewLevel(); 
                break;  
                case 3: 
                    levelTextObj.text = "Level 03"; 
                    gameState = GameState.gameOver; 
                    ResetForNewLevel(); 
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

    void ResetForNewLevel()
    {
        
        correctMatch = 0; 
        wrongMatch = 0; 
        correctMismatch = 0; 
        wrongMismatch = 0;
        missedMatch = 0; 
        missedMismatch = 0; 
        totalCorrectLetter = 0; 
        

        stimuli = 0; 

        switchedLevels = true; 
        usedLetters.Clear(); 


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
        {
            Debug.Log("Match missed! "); 
            CountScore(4); 
        }
            
        if( usedLetters.Count > n )
        {
            if(!inputHappened && currentLetter != correctLetter)
            {
                Debug.Log("Mismatch missed!");
                CountScore(5); 
            }
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

    public void CountScore(int i)
    {
        
        switch(i)
        {
            case 0: // Correct Match
                correctMatch++; 
            break; 
            case 1: // False Match
                wrongMatch++; 
            break; 
            case 2: // Correct Mismatch
                correctMismatch++; 
            break; 
            case 3: // False Mismatch
                wrongMismatch++; 
            break; 
            case 4: // Missed Match
                missedMatch++; 
            break; 
            case 5: // Missed Mismatch
                missedMismatch++; 
            break; 
        }
        
    }

    public void SaveScoreData()
    {
        
        levelData = new LevelData(); 
        
        levelData.level = currentLevel; 
        levelData.totalMatches = totalCorrectLetter; 
        levelData.totalMismatches = (stimuli-n) - totalCorrectLetter; 
        
        if(levelData.totalMatches > 0 )
        {
            levelData.correctlyMatched = (float)correctMatch / levelData.totalMatches; 
            levelData.missedMatches = (float)missedMatch / levelData.totalMatches; 

            levelData.falseAlarmMismatch = (float)wrongMismatch / levelData.totalMatches; 
        }
       
        
        if(levelData.totalMismatches > 0 )
        {
            
            levelData.correctlyMismatched = (float)correctMismatch / levelData.totalMismatches; 
            levelData.missedMismatches = (float)missedMismatch / levelData.totalMismatches;

            levelData.falseAlarm = (float)wrongMatch / levelData.totalMismatches;  
        }
            
        // Send levelData to server; Include User ID
        SQLSaveManager.instance.StartNBackPostCoroutine(levelData); 
        
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
                totalCorrectLetter++; 
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
                    Debug.Log("Correct Match"); 
                    CountScore(0); 
                }
                else
                {
                    Debug.Log("False Match");
                    CountScore(1);  
                }

            break; 
            //Mismatch Button 
            case 1: 
                if ( currentLetter == correctLetter)
                    {
                        Debug.Log("False Mismatch"); 
                        CountScore(3); 
                    }
                    else
                    {
                        Debug.Log("Correct Mismatch"); 
                        CountScore(2); 
                    }

            break; 
        }
    }

    
}
