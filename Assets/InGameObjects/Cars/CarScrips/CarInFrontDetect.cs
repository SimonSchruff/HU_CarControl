using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarInFrontDetect : SimulatedParent
{
    List<CarControlScript> colCars = new List<CarControlScript>();
    CarControlScript selfCar;

    private void Start()        //SetSelfCarRef
    {
        selfCar = GetComponentInParent<CarControlScript>();
    }

    #region TriggerEnter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (simState == simulationState.game)
        {
            ManualTriggerEnter(collision);
        }
    }

    public override void TriggerEnterSim(Collider2D otherCol)
    {
        if(simState == simulationState.simulated)
        {
            base.TriggerEnterSim(otherCol);
            ManualTriggerEnter(otherCol);
        }
    }
    void ManualTriggerEnter (Collider2D collision)
    {
        if (collision.CompareTag ("CarInner"))
        {
            if(collision.gameObject != gameObject.transform.parent.gameObject)  //Check if collision is not self
            {
                CarControlScript otherCar = collision.GetComponent<CarControlScript>();
                if(selfCar != null)
                {
                    selfCar.actualWaitingLightID = otherCar.actualWaitingLightID;
                    selfCar.carsInRowCounter = otherCar.carsInRowCounter + 1;

                    if (selfCar.actualWaitingLightID != 0)
                    {
                        if(simState == simulationState.simulated)
                        {
                            //       SimulationControlScript.sim.AddScoreToTrafficLight(selfCar.actualWaitingLightID, selfCar.carsInRowCounter);
                            int acWaID = selfCar.actualWaitingLightID;
                            foreach (TrafficLightScript tl in SimulationControlScript.sim.simTrafficLights)
                            {
                                if (tl.trafficLightID == acWaID)        //Set carsInRowInSim
                                {
                                    tl.waitingCarsCounter = selfCar.carsInRowCounter;
                                    break;
                                }
                            }

                        }
                        else 
                        {
                            SimulationControlScript.sim.GetTrafficLightRefFromID(selfCar.actualWaitingLightID).waitingCarsCounter = selfCar.carsInRowCounter;   //Set car in Row counter of Traffic light
                        }
                }
            }

                bool sameLane = false;

                if(selfCar == null)     //Check if car is set
                    selfCar = GetComponentInParent<CarControlScript>();

                foreach (int i in selfCar.pathID)       //CheckIf PathID is in other Car
                {
                    foreach (int o in otherCar.pathID)
                    {
                        if(i == o)
                        {
                            sameLane = true;
                            break;
                        }
                    }
                }

                if (sameLane)
                {
                    colCars.Add(otherCar);
                    selfCar.StopCarInFront(true);
                }
            }
        }
    }
    #endregion

    #region TriggerExit
    public override void TriggerExitSim(Collider2D otherCol)
    {
        if (simState == simulationState.simulated)
        {
            try { ManualExitTrigger(otherCol); }
            catch { }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (simState == simulationState.game && collision)
        {
            ManualExitTrigger(collision);
        }
    }
    private void ManualExitTrigger(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.CompareTag("CarInner"))
            {
                CarControlScript otherCar = collision.GetComponent<CarControlScript>();

                foreach (CarControlScript c in colCars)     // Check if car is in List
                {
                    if (c == otherCar)
                    {
                        selfCar.carsInRowCounter = 0;
                        selfCar.StopCarInFront(false);
                        break;
                    }
                }
            }
        }
        else
        {
            try
            {
                selfCar.StopCarInFront(false);
            }
            catch {}
        }
    }
    #endregion
}
