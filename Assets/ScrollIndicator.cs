using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Scrollbar))]
public class ScrollIndicator : MonoBehaviour
{
    private Scrollbar scrollbar;
    [SerializeField] private GameObject indicator;
    [SerializeField] private float value; 

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>(); 
    }

    void Update()
    {
        if(indicator != null)
        {
            if (scrollbar.value > value)
            {
                indicator.SetActive(false); 
            }
            else
            {
                indicator.SetActive(true); 
            }
        }
        else
        {
            Debug.LogError("Indicator UI Object not set!"); 
        }
        
    }

    public void GoToTop()
    {
        scrollbar.value = 0.9999f; 
    }
}
