using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 



public class FragebogenManager : MonoBehaviour
{  
    // Dictionary not able to be shown in inspector -> Workaround
    [Serializable]
    public struct Questions
    {
        public int id;
        public GameObject questionObj;  

    }
    public Questions[] sozQuestions;





   public void NextSozQuestion(int currentID)
   {
       sozQuestions[currentID].questionObj.gameObject.SetActive(false); 
       sozQuestions[currentID + 1].questionObj.gameObject.SetActive(true); 

   }
}
