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

    GameObject letterObject; 
    Text letterTextObj; 

    [Header("Variables")]

    float timer; 
    float hideTimer; 


    public float letterShowSec = 0.5f; 
    public float letterWaitSec = 2.5f; 

    //Letters that are randomly selected are directly assigned to List in inspector
    public List<string> lettersOfChoice = new List<string>(); 
    List<string> usedLetters = new List<string>(); 
    string currentLetter; 





    [Header("UI References")]
    public GameObject InstructionObjects; 
    public GameObject RunningGameObjects; 
    
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
            break; 

            case GameState.gameOver: 
            //Show Restart Button ? 
            //Save Score etc. 
            break; 
        }


        if(gameState == GameState.running)
        {

            timer += Time.deltaTime;  
            hideTimer += Time.deltaTime; 

            
            // TO DO: 

            // Show Numbers for .5s 
            // Black period in between of 2.5s 
            // Total of 3s for people to respond to number

            //Show Number
            //Save Current Letter, that is being shown and add to usedLetters list

            


            if ( timer > letterShowSec ) //Hide Letter after .5sec
            {
                
                timer = 0; 
                letterObject.SetActive(false); 

                if ( hideTimer >= letterWaitSec ) // Show new Letter after Wait Period 
                {
                    hideTimer = 0; 

                    letterTextObj.text = RandomizeLetter(); 
                    letterObject.SetActive(true); 
                }


            }

            

        }
        
    }

    string RandomizeLetter()
    {
        // Get Random Letter of list usedLetters
        // 33 % chance of correcct letter ( From 2 steps ago )

        string newLetter = "A"; 

        int rand = Random.Range(1,4); 
        
        switch(rand)
        {
            case 1: 
            case 2: 
                //show new random letter
                //newLetter = lettersOfChoice[Random.Range(0, lettersOfChoice.Count + 1)]; 
                newLetter = "B"; 
                //TO DO: Filter for random selection of correct letter
            break; 

            case 3: 
                Debug.Log("Correct Letter"); 
                newLetter = "A"; 
            break; 

            default: 
                newLetter = "C";
                break; 
        }

        return newLetter; 

        
    }

    void GetNewLetter()
    {
        letterTextObj.text = RandomizeLetter(); 
        currentLetter = letterTextObj.ToString(); 
        usedLetters.Add(currentLetter); 

         
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

    
}
