using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugTrafficScoreScript : MonoBehaviour
{
    [SerializeField] Text text1; 
    [SerializeField] Text text2;
    [SerializeField] Image image;
    [SerializeField] Color picked, notPicked; 


    public void ManualStart(string textOne, string textTwo , bool succes = false)
    {
            text1.text = textOne;
            text2.text = textTwo;
            StartCoroutine(Delete());

            image.color = succes ? picked : notPicked;  
    }

    IEnumerator Delete ()
    {
        yield return new WaitForSeconds (1);
        Destroy(gameObject);
    }
}
