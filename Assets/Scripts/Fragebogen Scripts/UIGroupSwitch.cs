using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGroupSwitch : MonoBehaviour
{
    [Header("Set Active for Groups")]
    [SerializeField] private GameObject group1;
    [SerializeField] private GameObject group2;
    [SerializeField] private GameObject group3;

    void Start()
    {
        switch(SQLSaveManager.instance.group)
        {
            case SQLSaveManager.Group.group1:
                group1.SetActive(true); 
               break;
            case SQLSaveManager.Group.group2:
                group2.SetActive(true);
                break;
            case SQLSaveManager.Group.group3:
                group3.SetActive(true);
                break;
        }
            
    }

    
}
