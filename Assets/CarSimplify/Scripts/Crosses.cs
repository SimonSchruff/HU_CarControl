using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Crosses : SimplifyParent
{
    public Crosses[] previousCrossH;
    public Crosses[] previousCrossV;

    public int [] TurnToLockSpwanPairs;
    public CarSpawnScript [] carSpawnLock;

    public bool actualState = false;
    public bool displayError = false;
    public bool locked = false;

    public SpriteRenderer highlightSprite;

    public int tempHighlightPrio;


    //public List<Vector2> crossedInTurns = new List<Vector2>();

    public List<KeyValuePair<bool, Vector2>> crossedInTurnsDictionary = new List<KeyValuePair<bool, Vector2>>(); 

    [SerializeField] Text priorityText;

    private void Start()
    {
        actualState = (Random.Range(0, 2) == 0);
        CrossClicked();
    }

    public override void SimpleUpdate()
    {
        base.SimpleUpdate();
    }

    public void ClearCrosses()
    {
        //crossedInTurns.Clear();
        crossedInTurnsDictionary.Clear();
    }

    public void CrossClicked ()
    {
        if(!locked)
        {
            actualState = !actualState;
            transform.rotation = Quaternion.Euler(0, 0, actualState ? 0 : 90);

            if (SimplAssis.instance.actualAssistance != SimplAssis.AssiState.auto)
            {
                SimplAssis.instance.UpdateAssistance();
            }
            // Add CrossesClickedCounter
            if (Control.instance.actualSaveClass != null)
            {
                Control.instance.actualSaveClass.amountUserClickedCrosses++;
            }
        }
        else
        {
            SimplAssis.instance.SetUpdateTimer();
        }
    }

    public void SetHighlighted(int priority = 0, bool setHighlighted = false)
    {
        //priorityText.gameObject.SetActive(setHighlighted);
        highlightSprite.gameObject.SetActive(setHighlighted);

        if (setHighlighted && priority != 0)
        {
            priorityText.text = priority.ToString();

            float tempScale = 0;
            float tempOpacity = 0;
            //Define visual stuff different Highlight things
            switch (priority)       
            {
                case 1:
                    tempScale = 6;
                    tempOpacity = 1;
                    break;
                case 2:
                    tempScale = 5;
                    tempOpacity =.85f;
                    break;
                case 3:
                    tempScale = 4.6f;
                    tempOpacity = .65f;
                    break;
                default:
                    tempScale = 4.3f;
                    tempOpacity = .5f;
                    break;
            }

            highlightSprite.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            var color = highlightSprite.color;
            highlightSprite.color = new Color(color.r, color.g, color.b, tempOpacity);
        }

    }
}
