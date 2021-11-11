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
        if (scrollbar.value <= 0f)
        {
            scrollbar.value = 0.0001f;
        }
        else if (scrollbar.value >= 1f)
        {
            scrollbar.value = 0.9999f;
        }
        else
        {

            if (!isInverted )
                scrollbar.value += Input.mouseScrollDelta.y * scrollSpeed;
            else
                scrollbar.value += Input.mouseScrollDelta.y * -scrollSpeed;
        }

        
    }
}
