using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class SliderPercentageIndicator : MonoBehaviour
{
    Slider slider;
    Text text;

    [SerializeField] private Text altText; 

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

        if(gameObject.name == "PercentageIndicator_com")
        {
            text.text = slider.value.ToString();
            altText.text = (100 - slider.value).ToString(); 
        }
    }
}
