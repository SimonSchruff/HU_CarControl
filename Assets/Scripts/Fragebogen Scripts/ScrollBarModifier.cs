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

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    void Update()
    {
        if (!isInverted)
            scrollbar.value += Input.mouseScrollDelta.y * scrollSpeed;
        else
            scrollbar.value += Input.mouseScrollDelta.y * -scrollSpeed;
    }
}
