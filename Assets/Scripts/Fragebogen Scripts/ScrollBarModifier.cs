using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollBarModifier : MonoBehaviour
{
    Scrollbar scrollbar;
    [Header("Variables")]
    public float scrollSpeed = 0.005f;
    public bool isInverted;

    // Set Navitgation mode to "NONE" in Inspector 

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    void Update()
    {
        Mathf.Clamp(scrollbar.value, 0, 1); 
        {
            //print(Input.mouseScrollDelta.y); 

            if (!isInverted )
            {

                Mathf.Clamp(scrollbar.value += Input.mouseScrollDelta.y * scrollSpeed ,0,1);   
            }
            else
            {
                Mathf.Clamp(scrollbar.value += Input.mouseScrollDelta.y * -scrollSpeed ,0,1);   
            }
        }

        
    }
}
