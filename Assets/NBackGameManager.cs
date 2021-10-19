using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class NBackGameManager : MonoBehaviour
{
    public static NBackGameManager Instance; 

    
    public enum GameState
    {
        instructions, 
        running, 
        gameOver
    }
    public GameState gameState; 

    
    
    
    [Header("Timer")]
    public float letterShowSec = 0.5f; 
    public float letterWaitSec = 2.5f; 
    float timer; 
    float hideTimer; 


    [Header("Letter Objects, Lists, and Variables")]
    public string currentLetter; 
    //Letters that are randomly selected are directly assigned to List in inspector
    public List<string> lettersOfChoice = new List<string>(); 
    public List<string> usedLetters = new List<string>(); 
    string correctLetter = "A";
    public bool inputHappened; 

    bool newLetterRdy = true; 



    [Header("UI References")]
    public GameObject InstructionObjects; 
    public GameObject RunningGameObjects; 
    GameObject letterObject; 
    Text letterTextObj; 

    
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

        letterObject = GameObject.FindGameObjectWithTag("letterObj"); 
        letterTextObj = letterObject.GetComponentInChildren<Text>(); 
    }

    void Update()
    {
        

        switch(gameState)
        {
            case GameState.instructions: 
            //Show Instructions UI
            InstructionObjects.SetActive(true); 
            RunningGameObjects.SetActive(false); 
            break; 

            case GameState.running: 
            InstructionObjects.SetActive(false); 
            RunningGameObjects.SetActive(true); 

            //Start Game Logic
            if(newLetterRdy == true)
                StartCoroutine(OneLetterPeriod()); 

            break; 

            case GameState.gameOver: 
            //Show Restart Button ? 
            //Save Score etc. 
            break; 
        }


        // OLD VERSION 
        /*
        if(gameState == GameState.running)
        {
            
            timer += Time.deltaTime;  
            hideTimer += Time.deltaTime;
            
            if ( timer > letterShowSec ) //Hide Letter after .5sec
            {
                
                timer = 0; 

                if(currentLetter == correctLetter && inputHappened == false)
                    Debug.Log("Correct Letter Missed"); 
                else if ( currentLetter != correctLetter && inputHappened == false )
                    Debug.Log(" No Input "); 

                letterObject.SetActive(false); 

                if ( hideTimer >= letterWaitSec ) // Show new Letter after Wait Period 
                {
                    hideTimer = 0; 

                    // If very first letter "correct" usedLetters.count - 2 is out of Range
                    if( usedLetters.Count > 1 )
                        correctLetter = usedLetters[usedLetters.Count - 2]; 

                    letterTextObj.text = RandomizeLetter(); 
                    currentLetter = letterTextObj.text; 
                    usedLetters.Add(currentLetter); 
                    
                    inputHappened = false; 
                    letterObject.SetActive(true); 
                }


            }

            

        }*/
        
    }

    IEnumerator OneLetterPeriod()
    {
        newLetterRdy = false; 

        // If very first letter "correct" usedLetters.count - 2 is out of Range
            if( usedLetters.Count > 1 )
                correctLetter = usedLetters[usedLetters.Count - 2]; 

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
        if(!inputHappened && currentLetter != correctLetter)
            Debug.Log("Mismatch missed!"); 


        newLetterRdy = true; 


    }

    string RandomizeLetter()
    {
        string newLetter = "A"; 
     
        // Create 1/3 chance of "correct" letter 
        int rand = Random.Range(1,4); 
        if(usedLetters.Count < 2 )
            rand = 1; 

        switch(rand)
        {
            case 1: 
            case 2: 
                //Create new random letter
                string tempLetter = "A"; 
                tempLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count)]; 
                // If random letter is the same as correct letter, repeat randomizing until different letter
                while( tempLetter  == correctLetter )
                {
                    tempLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count)]; 
                }
                newLetter = tempLetter; 
            break; 

            case 3: 
                //Show "Correct" Letter
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
            gameState = GameState.instructions; 
            break; 
            case 1 : 
            gameState = GameState.running; 
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
