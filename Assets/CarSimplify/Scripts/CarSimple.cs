using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSimple : SimplifyParent
{
    float gridSize;
    float spawnDelay;

    Crosses[] crossLane;
    bool horOrVert;

    private void Start()
    {
        gridSize = Control.con.gridSize;
        spawnDelay = Control.con.spawnDelay;
    }

    IEnumerator DestroyAfter(float time)
    {
        yield return new WaitForSeconds(time);
        DestroyCar(true);
    }

    public void ManualInit (Crosses [] CrossLane, bool HorOrVertical)
    {
        crossLane = CrossLane;
        horOrVert = HorOrVertical;
        actualStep = -1;  //Set on -1 to update on Spawn and set on 0

        spawnDelay = Control.con.spawnDelay;
        StartCoroutine(DestroyAfter(horOrVert ? 7 * spawnDelay : 4 * spawnDelay));
    }
    public void FillCrosses()
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

    private void Update()
    {
        gameObject.transform.Translate(new Vector3((gridSize * Time.deltaTime)/spawnDelay, 0, 0));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("CrossInner"))
        {
            Crosses cross = collision.GetComponentInParent<Crosses>();
            if(cross.actualState == horOrVert)
            {
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
        ScoreSimple.sco.UpdateScore(success ? 5 : -3);

        Destroy(gameObject);
    }

}
