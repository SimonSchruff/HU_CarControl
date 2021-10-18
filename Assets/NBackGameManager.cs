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
        
    }
}
