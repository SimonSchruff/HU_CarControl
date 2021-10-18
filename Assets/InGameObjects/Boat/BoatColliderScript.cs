using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatColliderScript : SimulatedParent
{
    BridgeScript bridge;
    bool simStartOverlapping = false;

    private void Awake()
    {
        ManualAwake();
    }

    public override void InitSimulation()
    {
        base.InitSimulation();
        ManualAwake();

        simStartOverlapping = true;
    }

    public override void UpdateSimulation(float simStep)
    {
        base.UpdateSimulation(simStep);
        if (simStartOverlapping)
            simStartOverlapping = false;
    }

    void ManualAwake ()
    {
        bridge = GetComponentInParent<BridgeScript>();
    }

    #region TriggerEnter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (simState == simulationState.game)
        {
            if (collision.CompareTag("Boat"))
            {
                bridge.ChangeBridgeState(BridgeScript.BridgeState.opened);
            }
        }
    }
    public override void TriggerEnterSim(Collider2D otherCol)
    {
        base.TriggerEnterSim(otherCol);
        if(!simStartOverlapping)
        {
            if (otherCol.CompareTag("Boat"))
            {
                bridge.ChangeBridgeState(BridgeScript.BridgeState.opened);
            }
        }
    }
#endregion
#region TriggerExit
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (simState == simulationState.game)
        {
            if (collision.CompareTag("Boat"))
            {
                bridge.ChangeBridgeState(BridgeScript.BridgeState.closed);
            }
        }
    }
    public override void TriggerExitSim(Collider2D otherCol)
    {
        base.TriggerExitSim(otherCol);

        if (otherCol.CompareTag("Boat"))
        {
            bridge.ChangeBridgeState(BridgeScript.BridgeState.closed);
        }
    }
}
#endregion
