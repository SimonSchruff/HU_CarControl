using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreSimple : MonoBehaviour
{
    [SerializeField] Text scoreText;

    public static ScoreSimple sco;

    int score = 0;

    private void Awake()
    {
        if(sco == null)
        {
            sco = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void UpdateScore(int amount)
    {
        if (amount!=0)
        {
            score += amount;
            scoreText.text = score.ToString();

            scoreText.GetComponent<Animator>().SetTrigger(amount > 0 ? "p" : "n");
        }
    }
    public int GetScore ()
    {
        return score;
    }
    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString(); 
    }

    public void ChangeScoreVisibility (bool ChangeVisibilityTo)
    {
       scoreText.gameObject.SetActive(ChangeVisibilityTo);
    }    
}
