using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SpotSimScript : SimulatedParent
{
    [SerializeField] bool editorPositioning = false;
    [SerializeField] Text textRef;
    public int id;

    public float spotSize = 1f;
    CircleCollider2D col;
    public List<TrafficLightScript> trafficLights = new List<TrafficLightScript>();
    SpriteRenderer sprite;

    private void Awake()
    {
        UpdateColRadius();
    }

    public override void InitSimulation()
    {
        base.InitSimulation();
        UpdateColRadius();
    }

    private void UpdateColRadius()
    {
        col = GetComponent<CircleCollider2D>();

        col.radius = spotSize / 2;

        if (simState == simulationState.simulated)
        {
        }

        List<Collider2D> otherCol = new List<Collider2D>();
        col.OverlapCollider(filterCollision, otherCol);

        trafficLights.Clear();


        TrafficLightScript tls;
        foreach (Collider2D collider in otherCol)
        {

            if (collider.gameObject.TryGetComponent(out tls))
            {
                trafficLights.Add(tls);
            } 
        }
        if (editorPositioning)
            col.enabled = true;
        else
            col.enabled = simState == simulationState.game ? false : true;

        try
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
            float temp = spotSize*1.75f;
            sprite.gameObject.transform.localScale = new Vector3(temp,temp,temp);
        }
        catch {}
        ChangeText("");

    }

    public override void UpdateSimulation(float simStep)
    {
        base.UpdateSimulation(simStep);
        if(trafficLights.Count == 0)
        {
            UpdateColRadius();
        }
    }

    public void HighlightSpot (float intensity0bis1)
    {

    }

    public void ChangeText (string text)
    {
        if (textRef != null)
            textRef.text = text;
    }
}
