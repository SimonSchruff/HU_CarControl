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
        public float totalCorrectPercentage; 
        public float correctlyMatched; 
        public int totalCorrectMatches; 
        public float falseAlarm; 
        public int totalFalseAlarms; 
        public float correctlyMismatched;
        public int totalCorrectMismatches;  
        public float falseAlarmMismatch;
        public int totalFalseAlarmMismatches; 
        public float missedMatches; 
        public int totalMissedMatches; 
        public float missedMismatches; 
        public int totalMissedMismatches; 
        //public float totalMatches;  
        //public float totalMismatches;
        
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
    public float matchProbability = 0.3333333333f;   
    
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
    int totalCorrectLetter; 
    public int totalMatchesPerRound;  
    public int currentMatchesPerRound; 
    




    [Header("UI References")]
    public GameObject InstructionObjects; 
    public GameObject InstructionsObjExample; 
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

        //Calculate Total correct matches
        float tempFloat =  (float)stimuliShown * matchProbability;
        totalMatchesPerRound = Mathf.RoundToInt(tempFloat); 
        Debug.Log("Total Matches per Round: "+totalMatchesPerRound) ;
        currentMatchesPerRound = totalMatchesPerRound; 

        gameState = GameState.instructions; 
        InstructionObjects.SetActive(true); 
    }

    void Update()
    {
        

        switch(gameState)
        {
            case GameState.timer: 
                TimerGameObjects.SetActive(true); 
                InstructionsObjExample.SetActive(false);  
                RunningGameObjects.SetActive(false);
                LevelFinishedGameObjects.SetActive(false); 
                GameOverObjects.SetActive(false); 
                
                if(newTimerRdy)
                    StartCoroutine(Timer()); 
            break; 

            case GameState.instructions: 
                TimerGameObjects.SetActive(false); 
                RunningGameObjects.SetActive(false); 
                LevelFinishedGameObjects.SetActive(false); 
                GameOverObjects.SetActive(false); 
            break; 

            case GameState.running: 
                TimerGameObjects.SetActive(false);
                InstructionObjects.SetActive(false); 
                InstructionsObjExample.SetActive(false); 
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
                    scoreUINumbers[0].text = totalMatchesPerRound.ToString(); 
                    scoreUINumbers[1].text = levelData.correctlyMatched.ToString(); 
                    scoreUINumbers[2].text = levelData.falseAlarm.ToString(); 
                    scoreUINumbers[3].text = levelData.missedMatches.ToString(); 
                    //Mismatch Numbers
                    scoreUINumbers[4].text = totalMatchesPerRound.ToString(); 
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
                case 0: //Round 1 of 4
                break; 
                case 1: //Round 2 of 4
                    gameState = GameState.levelFinished; 
                    levelTextObj.text = "Level 2 of 4"; 
                    ResetForNewLevel(); 
                break; 
                case 2: //Round 3 of 4
                    levelTextObj.text = "Level 3 of 4"; 
                    gameState = GameState.levelFinished;
                    ResetForNewLevel(); 
                break;  
                case 3: //Round 4 of 4
                    levelTextObj.text = "Level 4 of 4"; 
                    gameState = GameState.levelFinished; 
                    ResetForNewLevel(); 
                break; 
                case 4: // Game Over
                    gameState = GameState.gameOver; 
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

    IEnumerator WaitForSeconds(float s)
    {
        yield return new WaitForSeconds(s); 
    }

    public void SwitchInstructionsPage(int i)
    {
        

        switch(i)
        {
            case 1: //Instructions
                InstructionObjects.SetActive(true); 
                InstructionsObjExample.SetActive(false); 
            break;
            case 2: //Example
                InstructionObjects.SetActive(false); 
                InstructionsObjExample.SetActive(true); 
            break; 
            

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

        currentMatchesPerRound = totalMatchesPerRound; 
        

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
        float totalMismatches = (stimuli-n) - totalMatchesPerRound; 
        //levelData.totalMismatches = (stimuli-n) - totalMatchesPerRound;

        //Numeric Int amount of Matches etc. Per Round
        levelData.totalCorrectMatches = correctMatch; 
        levelData.totalCorrectMismatches = correctMismatch ;

        levelData.totalFalseAlarms = wrongMatch; 
        levelData.totalFalseAlarmMismatches = wrongMismatch; 

        levelData.totalMissedMatches = missedMatch; 
        levelData.totalMissedMismatches = missedMismatch; 
        
        //Percentages of correctly identified matches etc. per Round
        if(totalMatchesPerRound > 0 )
        {
            levelData.correctlyMatched = (float)correctMatch / (float)totalMatchesPerRound; 
            levelData.missedMatches = (float)missedMatch / (float)totalMatchesPerRound; 

            levelData.falseAlarmMismatch = (float)wrongMismatch / (float)totalMatchesPerRound; 
        }

        if(totalMismatches > 0 )
        {
            
            levelData.correctlyMismatched = (float)correctMismatch / (float)totalMismatches; 
            levelData.missedMismatches = (float)missedMismatch / (float)totalMismatches;

            levelData.falseAlarm = (float)wrongMatch / (float)totalMismatches;  
        }

        //Total Percentage score of correctly identified Matches and Mismatches
        levelData.totalCorrectPercentage = ((float)correctMatch + (float)correctMismatch) / (float)(stimuliShown-n); 
        Debug.Log(levelData.totalCorrectPercentage); 
            
        // Send levelData to server; Include User ID
        //SQLSaveManager.instance.StartNBackPostCoroutine(levelData); 
        
    }
    

    string RandomizeLetter()
    {
        string newLetter = "X"; 
        
        /*
            ---- HANDLE EDGE CASES FOR PROBABILITY ----
            - First two letters can`t be match/mismatch
            - If amount of matches per current round already shown, dont show correct letter
            - If number of remaining stimuli is <= matches still to be shown, show them at the end
        */

        int rand; 
        if(usedLetters.Count <= n )
            rand = 1; 
        else if(currentMatchesPerRound == 0)
            rand = 1; 
        else if((stimuliShown - usedLetters.Count ) <=  currentMatchesPerRound)
            rand = 3;
        else
            rand = Random.Range(1,4); 


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
                currentMatchesPerRound--; 
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
