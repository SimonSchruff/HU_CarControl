using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class MySceneManager : MonoBehaviour
{
    public static MySceneManager Instance; 
    
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
    }

    public void LoadSceneInt(int i)
    {

        SceneManager.LoadScene(i); 
        
    }

    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name); 
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }
   
}
