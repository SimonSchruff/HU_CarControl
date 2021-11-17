using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SliderPercentageIndicator : MonoBehaviour
{
    Slider slider;
    Text text; 

    void Start()
    {
        if (GetComponentInParent<Slider>() != null)
            slider = GetComponentInParent<Slider>();
        else
            Debug.LogError("No Slider in Parent Found");

        text = GetComponent<Text>(); 

    }

    void Update()
    {
        text.text = slider.value.ToString(); 
    }
}
