using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosses : SimplifyParent
{
    public Crosses[] previousCrossH;
    public Crosses[] previousCrossV;

    public int [] TurnToLockSpwanPairs;
    public CarSpawnScript [] carSpawnLock;

    public bool actualState = false;

    public bool locked = false;

    public SpriteRenderer highlightSprite;

    public int tempHighlightPrio;


    public List<Vector2> crossedInTurns = new List<Vector2>();

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
        crossedInTurns.Clear();
    }

    public void CrossClicked ()
    {
        if(!locked)
        {
            actualState = !actualState;
            transform.rotation = Quaternion.Euler(0, 0, actualState ? 0 : 90);

            if (SimplAssis.assi.actualAssistance != SimplAssis.assiState.auto)
            {
                SimplAssis.assi.UpdateAssistance();
            }
            // Add CrossesClickedCounter
            if (Control.con.actualSaveClass != null)
            {
                Control.con.actualSaveClass.amountUserClickedCrosses++;
            }
        }
        else
        {
            SimplAssis.assi.SetUpdateTimer();
        }
    }

    public void SetHighlighted(int priority = 0, bool setHighlighted = false)
    {
    //    priorityText.gameObject.SetActive(setHighlighted);
        highlightSprite.gameObject.SetActive(setHighlighted);

        if (setHighlighted && priority != 0)
        {
            priorityText.text = priority.ToString();

            float tempScale = 0;
            float tempOpa = 0;
            switch (priority)       //Define visual stuff different Highlight things
            {
                case 1:
                    tempScale = 6;
                    tempOpa = 1;
                    break;
                case 2:
                    tempScale = 5;
                    tempOpa =.85f;
                    break;
                case 3:
                    tempScale = 4.6f;
                    tempOpa = .65f;
                    break;
                default:
                    tempScale = 4.3f;
                    tempOpa = .5f;
                    break;
            }

            highlightSprite.transform.localScale = new Vector3(tempScale, tempScale, tempScale);
            highlightSprite.color = new Color(highlightSprite.color.r, highlightSprite.color.g, highlightSprite.color.b, tempOpa);
        }

    }
}
