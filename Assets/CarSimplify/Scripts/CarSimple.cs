using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// TODO: Add Different vehicle types as inherited classes
public class CarSimple : SimplifyParent
{
    // Set in Prefab
    // Enum determines how car is going to be saved; 
    public enum CarType
    {
        car00, 
        car01, 
        car02
    }
    public CarType carType;

    [Header("Assistance Error Variables")] 
    [Tooltip("Determines whether or not the assistance system can miss the car;")] public bool CanBeMissedByAssiSystem = false;
    [Tooltip("Determines how likely it is that the assistance system will miss the car;")] public float ErrorProbability = 0.7f; 
    
    float gridSize;
    float spawnDelay;

    Crosses[] crossLane;
    bool horOrVert;

    bool died = false;

    private void Start()
    {
        gridSize = Control.instance.gridSize;
        spawnDelay = Control.instance.spawnDelay;
    }

    private IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyCar(true);
    }
    private IEnumerator AddPointsAfter(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponentInChildren<Animator>().SetTrigger("suc");
        ScoreSimple.sco.UpdateScore (5);
    }


    public void ManualInit (Crosses[] CrossLane, bool HorOrVertical)
    {
        crossLane = CrossLane;
        horOrVert = HorOrVertical;
        actualStep = -1;  //Set on -1 to update on Spawn and set on 0

        // Get Score after timer is over
        spawnDelay = Control.instance.spawnDelay;
        StartCoroutine(DestroyAfter(horOrVert ? 7 * spawnDelay : 4 * spawnDelay));
        StartCoroutine(AddPointsAfter(horOrVert ? (5 * spawnDelay) +(.25f * spawnDelay) : (3 * spawnDelay) + (.30f * spawnDelay)));
    }
    public void FillCrosses()
    {
        if (died)
            return;
        
        // Tell crosses in how many steps car will need to pass
        for(int i = actualStep; i < crossLane.Length; i++)
        {
            if (crossLane[i] != null)
            {
                bool isError = false;
                if (CanBeMissedByAssiSystem) 
                {
                    float randProbability = Random.Range(0f, 1f);
                    if (randProbability < ErrorProbability) { isError = true; }
                }
                
                crossLane[i].crossedInTurnsDictionary.Add(new KeyValuePair<bool, Vector2>(isError, new Vector2(i - actualStep, horOrVert ? 0 : 1)));
            }
        }
    }

    private void Update()
    {
        if(!died)
        {
            gameObject.transform.Translate(new Vector3((gridSize * Time.deltaTime)/spawnDelay, 0, 0));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("CrossInner"))
        {
            Crosses cross = collision.GetComponentInParent<Crosses>();
            if(cross.actualState == horOrVert)
            {
                if (Control.instance.actualSaveClass != null)
                {
                    Control.instance.actualSaveClass.crossesCrossed++;

                    if(horOrVert)
                        Control.instance.actualSaveClass.crossesCrossedHorizontal++;
                    else
                        Control.instance.actualSaveClass.crossesCrossedVertical++;
                }

                cross.locked = true;
            }
            else
            {
                DestroyCar();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("CrossInner"))
        {
            collision.GetComponentInParent<Crosses>().locked = false;
        }
    }

    private void DestroyCar(bool success = false)
    {
        /*
        if(!success)     // Score is added earlier 
            ScoreSimple.sco.UpdateScore(success ? 5 : -3);
        */
        
        if (!success)
        {
            died = true;
            ScoreSimple.sco.UpdateScore(-3);
            SimplAssis.instance.UpdateAssistance();

            GetComponentInChildren<Animator>().SetTrigger("Die");

            StartCoroutine(DestroyAfterTime());

            if (Control.instance.actualSaveClass != null)
            {
                Control.instance.actualSaveClass.carsCrashedTotal++;
                switch (carType)
                {
                    case CarType.car00:
                        Control.instance.actualSaveClass.cars00Crashed++;
                        break;
                    case CarType.car01:
                        Control.instance.actualSaveClass.cars01Crashed++;
                        break;
                    case CarType.car02:
                        Control.instance.actualSaveClass.cars02Crashed++;
                        break;
                }
            }
        }
        else
        {
            // Score is added in CarSimple.AddPointsAfter()
            Destroy(gameObject);
            if (Control.instance.actualSaveClass != null)
            {
                Control.instance.actualSaveClass.carsSuccessTotal++;
                switch (carType)
                {
                    case CarType.car00:
                        Control.instance.actualSaveClass.cars00Success++;
                        break;
                    case CarType.car01:
                        Control.instance.actualSaveClass.cars01Success++;
                        break;
                    case CarType.car02:
                        Control.instance.actualSaveClass.cars02Success++;
                        break;
                }
            }
        }

    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2); //Destroy after 1s
        Destroy(gameObject);
    }
}
