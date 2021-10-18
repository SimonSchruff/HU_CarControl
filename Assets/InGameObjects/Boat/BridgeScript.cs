using System.Collections.Generic;
using UnityEngine;

public class BridgeScript : SimulatedParent
{
    public BridgeState state = BridgeState.closed;
    [SerializeField] SpriteRenderer bridgeSprite;
    public ContactFilter2D contactFilter;
    Color startColor;
    public int collidingBoatsCounter = 0;

    private void Awake()
    {
        startColor = bridgeSprite.color;
    }
    public enum BridgeState
    {
        opened,
        closed
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(simState == simulationState.game)
        {
            if (state == BridgeState.opened)
            {
                if (collision.CompareTag("CarInner"))
                {
                    collision.GetComponent<CarControlScript>().CrashHappened(CarControlScript.crashState.inWater);
                }
            }
        }
    }

    public override void TriggerEnterSim(Collider2D otherCol)
    {
        base.TriggerEnterSim(otherCol);
        
        if (state == BridgeState.opened)
        {
            if (otherCol.CompareTag("CarInner"))
            {
                otherCol.GetComponent<CarControlScript>().CrashHappened(CarControlScript.crashState.inWater);
            }
        }
    }

    public void ChangeBridgeState(BridgeState newState)
    {
        collidingBoatsCounter = newState == BridgeState.opened ? ++collidingBoatsCounter : --collidingBoatsCounter;
        if (newState != state)  //Check if state is different
        {
            if(newState == BridgeState.opened) // Bridge was closed and opens for boat
            {
                BoxCollider2D tc = GetComponent<BoxCollider2D>();
                List<Collider2D> tempColliders = new List<Collider2D>();
                tc.OverlapCollider(contactFilter, tempColliders);
                foreach (Collider2D c in tempColliders)
                {
                    if (c.CompareTag("CarInner"))
                    {
                        c.GetComponent<CarControlScript>().CrashHappened(CarControlScript.crashState.inWater);
                    }
                }

                state = newState;

                bridgeSprite.gameObject.SetActive(false);       // What to do when bridge opens
            }
            else // Bridge is opened and closes 
            {
                if(collidingBoatsCounter == 0)      //What to do when no boat is colliding any more
                {
                    bridgeSprite.gameObject.SetActive(true);
                    bridgeSprite.color = startColor;
                    state = newState;
                }

            }


        }

    }
}
