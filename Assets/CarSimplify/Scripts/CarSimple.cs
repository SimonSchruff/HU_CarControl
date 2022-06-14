using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimple : SimplifyParent
{
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

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyCar(true);
    }
    IEnumerator AddPointsAfter(float time)
    {
        yield return new WaitForSeconds(time);
        GetComponentInChildren<Animator>().SetTrigger("suc");
        ScoreSimple.sco.UpdateScore (5);
    }


    public void ManualInit (Crosses [] CrossLane, bool HorOrVertical)
    {
        crossLane = CrossLane;
        horOrVert = HorOrVertical;
        actualStep = -1;  //Set on -1 to update on Spawn and set on 0

        spawnDelay = Control.instance.spawnDelay;
        StartCoroutine(DestroyAfter(horOrVert ? 7 * spawnDelay : 4 * spawnDelay));
        StartCoroutine(AddPointsAfter(horOrVert ? (5 * spawnDelay) +(.25f * spawnDelay) : (3 * spawnDelay) + (.30f * spawnDelay)));
    }
    public void FillCrosses()
    {
        if (!died)
        {
            int counter = -1;

            for (int i = actualStep; i < crossLane.Length; i++)
            {
                counter++;

                if(crossLane[i] != null)
                {
                    crossLane[i].crossedInTurns.Add(new Vector2(i - actualStep, horOrVert ? 0 : 1));
                }
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

    void DestroyCar(bool success = false)
    {
        if(!success)     // Score is added earlier 
            ScoreSimple.sco.UpdateScore(success ? 5 : -3);

        if (!success)
        {
            died = true;
            SimplAssis.instance.UpdateAssistance();

            GetComponentInChildren<Animator>().SetTrigger("Die");

            StartCoroutine(DestroyAfterTime());

            if (Control.instance.actualSaveClass != null)
            {
                Control.instance.actualSaveClass.carsCrashed++;
            }
        }
        else
        {
            Destroy(gameObject);
            if (Control.instance.actualSaveClass != null)
            {
                Control.instance.actualSaveClass.carsSuccess++;
            }
        }

    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(2); //Destroy after 1s
        Destroy(gameObject);
    }
}
