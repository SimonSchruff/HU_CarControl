using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplifyParent : MonoBehaviour
{
    public int actualStep = 0;

    public virtual void SimpleUpdate ()
    {
        actualStep++;
    }
}
