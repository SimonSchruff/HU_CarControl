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
        public int currentLevel; 
        public int totalCorrectMatches; 
        public int totalCorrectMismatches;  
        public int totalFalseDecisionMatch; 
        public int totalFalseDecisionMismatch; 
        public int totalNoReactionMatches; 
        public int totalNoReactionMismatches; 
        public float totalCorrectPercentage; 
        public float correctlyMatched; 
        public float correctlyMismatched;
        public float falseDecisionMatch; 
        public float falseDecisionMismatch;
        public float noReactionMatches; 
        public float noReactionMismatches; 
    }
    public LevelData levelData; 

    enum GameState
    {
        timer,
        instructions, 
        running, 
        levelFinished, 
        gameOver
    }
    GameState gameState; 


    [Header("Variables")]
    public int n = 2;
    public float letterShowSec = 0.5f; 
    public float letterWaitSec = 2.5f; 
    
    
    [Header("Round Stimuli Lists")]
    public List<string> round1Numbers = new List<string>();
    public List<string> round2Numbers = new List<string>();
    //public List<string> round3Numbers = new List<string>();
    //public List<string> round4Numbers = new List<string>();
    List<string> usedLetters = new List<string>();


    // Private Varriables
    int currentLevel = 0; 
    string currentLetter; 
    string correctLetter = "A";
    int stimuli = 0; 
    int stimuliShown = 0;
   

    #region Bools
    bool inputHappened; 
    bool newLetterRdy = true; 
    bool newTimerRdy = true; 
    bool switchedLevels = false; 
    #endregion

    #region Score
    int correctMatch; 
    int wrongMatch; 
    int correctMismatch; 
    int wrongMismatch;
    int missedMatch; 
    int missedMismatch;  
    #endregion

    #region UIRefs 
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
    #endregion

    
    
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
        InstructionObjects.SetActive(true);

        stimuliShown = round1Numbers.Count; 
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
                InstructionObjects.SetActive(true);
                LevelFinishedGameObjects.SetActive(false); 
                GameOverObjects.SetActive(false); 
            break; 

            case GameState.running: 
                TimerGameObjects.SetActive(false);
                InstructionObjects.SetActive(false); 
                RunningGameObjects.SetActive(true);
                LevelFinishedGameObjects.SetActive(false);
                GameOverObjects.SetActive(false);   

                

                if(stimuli >= round1Numbers.Count && currentLevel == 0 || stimuli >= round2Numbers.Count && currentLevel == 1)
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
                    levelTextObj.text = "Round 2 of 2"; 
                    ResetForNewLevel(); 
                break; 
                case 2: //Round 3 of 4
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


    void ResetForNewLevel()
    {
        usedLetters.Clear(); 

        correctMatch = 0; 
        wrongMatch = 0; 
        correctMismatch = 0; 
        wrongMismatch = 0;
        missedMatch = 0; 
        missedMismatch = 0; 
        
        stimuli = 0; 
        stimuliShown = round2Numbers.Count; 

        switchedLevels = true; 
     
    }

    IEnumerator OneLetterPeriod()
    {
        newLetterRdy = false;
        inputHappened = false;  
     
        // Correct letter only possible if usedLetters.Count is >= then n 
        if( usedLetters.Count >= n )
        {
            correctLetter = usedLetters[usedLetters.Count - n]; 
        }
        
        //Add Correct List according to level
        if(currentLevel == 0)
        {
            letterTextObj.text = round1Numbers[stimuli]; 
        }
        else if(currentLevel == 1)
        {
            letterTextObj.text = round2Numbers[stimuli];
        }
        
        
        currentLetter = letterTextObj.text; 
        usedLetters.Add(currentLetter); // Add to used Letter List


        //SET UI
        letterObject.SetActive(true); 

        yield return new WaitForSeconds(letterShowSec); 

        letterObject.SetActive(false); 

        yield return new WaitForSeconds(letterWaitSec); 

        // If no Input happened during full letter period
        if( usedLetters.Count > n ) // > instead of >= because letter has already been added to List above
        {
            if(!inputHappened && currentLetter == correctLetter)
            {
                Debug.Log("Match missed! "); 
                CountScore(4); 
            }
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

        // Count Down
        timerTextObj.text = "3"; 
        yield return new WaitForSeconds(1); 
        timerTextObj.text = "2";    
        yield return new WaitForSeconds(1); 
        timerTextObj.text = "1"; 
        yield return new WaitForSeconds(1);
        timerTextObj.text = "GO";
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
        
        levelData.currentLevel = currentLevel; 
        float totalMatches = correctMatch + missedMatch + wrongMismatch;  
        float totalMismatches = (stimuliShown-n) - totalMatches; 
        
        //Numeric Int amount of Matches etc. Per Round
        levelData.totalCorrectMatches = correctMatch; 
        levelData.totalCorrectMismatches = correctMismatch ;

        levelData.totalFalseDecisionMatch = wrongMatch; 
        levelData.totalFalseDecisionMismatch = wrongMismatch; 

        levelData.totalNoReactionMatches = missedMatch; 
        levelData.totalNoReactionMismatches = missedMismatch; 
        
        //Percentages of correctly identified matches etc. per Round
        if(totalMatches > 0 )
        {
            levelData.correctlyMatched = (float)correctMatch / (float)totalMatches; 
            levelData.noReactionMatches = (float)missedMatch / (float)totalMatches; 

            levelData.falseDecisionMismatch = (float)wrongMismatch / (float)totalMatches; 
        }

        if(totalMismatches > 0 )
        {
            
            levelData.correctlyMismatched = (float)correctMismatch / (float)totalMismatches; 
            levelData.noReactionMismatches = (float)missedMismatch / (float)totalMismatches;

            levelData.falseDecisionMatch = (float)wrongMatch / (float)totalMismatches;  
        }

        //Total Percentage score of correctly identified Matches and Mismatches
        levelData.totalCorrectPercentage = ((float)correctMatch + (float)correctMismatch) / (float)(stimuliShown-n);

        SQLSaveManager.instance.SaveNBackData(levelData); 
        
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
