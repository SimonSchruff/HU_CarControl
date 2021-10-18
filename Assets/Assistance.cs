using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assistance : MonoBehaviour
{
    public static Assistance assi;

    public AssistanceTypes actualAssistance;

    public enum AssistanceTypes
    {
        Manual,
        Semi,
        Full
    }


    private void Awake()
    {
        if (assi == null)
        {
            assi = this;
        }
        else
            Destroy(this.gameObject);
    }
}
