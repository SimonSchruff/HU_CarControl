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
        }
        else
        {
            SimplAssis.assi.SetUpdateTimer();
        }
    }

    public void SetHighlighted(int priority = 0, bool setHighlighted = false)
    {
        priorityText.gameObject.SetActive(setHighlighted);
        highlightSprite.gameObject.SetActive(setHighlighted);

        if (setHighlighted && priority != 0)
            priorityText.text = priority.ToString();

        priorityText.text = tempHighlightPrio.ToString();
    }
}
