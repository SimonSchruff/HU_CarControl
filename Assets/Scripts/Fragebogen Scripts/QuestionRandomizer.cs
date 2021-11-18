using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class QuestionRandomizer : MonoBehaviour
{
    private GameObject[] answers;

    void Start()
    {
        AnswerSaver[] temp = GetComponentsInChildren<AnswerSaver>();
        answers = new GameObject[temp.Length]; 
        for(int i = 0; i < temp.Length; i++)
        {
            answers[i] = temp[i].gameObject; 
        }

        RandomizeQuestions(answers); 
    }

    //Inspector Button Event
    public void RandomizeOnButtonPress()
    {
        RandomizeQuestions(answers); 
    }

    // Shuffle Position of Questions
    void RandomizeQuestions(GameObject[] a)
    {
        for (int i = 0; i < a.Length; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, a.Length);
            GameObject temp = a[i];
            Vector3 pos = new Vector3(a[i].transform.localPosition.x, a[i].transform.localPosition.y, 0);
            Vector3 alternatePos = new Vector3(a[randomIndex].transform.localPosition.x, a[randomIndex].transform.localPosition.y, 0);

            //Swaps Position of GameObjects
            a[i].transform.localPosition = alternatePos;
            a[randomIndex].transform.localPosition = pos; 
        }
    }
    
    
}


