using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class RandSceneLoader : MonoBehaviour
{
    public static RandSceneLoader instance;
    
    [Tooltip("Build Index of all scenes that are loaded randomly;")] public int[] randSceneBuildIndex = new int[1];
    [Tooltip("Build Index of scene that is loaded after all random scenes have been active;")] public int sceneToLoadAfterRandScenes; 

    private List<int> _randScenesToLoad = new List<int>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
        DontDestroyOnLoad(gameObject);
        
        // Fill List with build indices set in inspector
        for(int i = 0; i < randSceneBuildIndex.Length; i++)
        { 
            _randScenesToLoad.Add(randSceneBuildIndex[i]);
        }
    }

    /// <summary>
    /// Load Scene randomly from specified build indexes; 
    /// </summary>
    public void LoadSceneRandomly()
    {
        print("Count: " + _randScenesToLoad.Count);
        if (_randScenesToLoad.Count == 0)
        {
            SceneManager.LoadScene(sceneToLoadAfterRandScenes);
            return;
        }
        
        //print("Random Load Rand: " + rand);
        int rand = Random.Range(0, _randScenesToLoad.Count); 
        
        SceneManager.LoadScene(_randScenesToLoad[rand]);
        _randScenesToLoad.Remove(_randScenesToLoad[rand]);
    }
}
