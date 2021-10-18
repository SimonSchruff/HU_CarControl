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

    //Letters are directly assigned to List in inspector
    public List<string> usedLetters = new List<string>(); 

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
            //Disable Instructions UI 
            //Enable Game UI 
            //Start Game Logic
            break; 

            case GameState.gameOver: 
            //Show Restart Button ? 
            //Save Score etc. 
            break; 
        }
        
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
