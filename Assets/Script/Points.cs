using System;
using UnityEngine;
using UnityEngine.UI;

public class Points : MonoBehaviour
{
    [SerializeField]
    private Slider Slider;

    [SerializeField]
    private Text ScoreText;

    [SerializeField]
    private int BaseGoalScore;

    [SerializeField]
    private GameManager GameManager;

    public void SetScore(int newScore)
    {
        Slider.value += newScore;
        SetScoreText((int)Slider.value);
    }

    public void CalculateGoalScore(int round)
    {
        double goalScore = BaseGoalScore + Math.Pow(round, 1.5);
        SetRoundGoal((int)goalScore);
    }

    public void ChangeScore(int numberOfDestroyedGems, int minimumMatchNumber)
    {
        if (numberOfDestroyedGems == minimumMatchNumber)
        {
            SetScore(numberOfDestroyedGems * 2);
        }
        else if (numberOfDestroyedGems > minimumMatchNumber && numberOfDestroyedGems <= 2 * minimumMatchNumber)
        {
            SetScore(numberOfDestroyedGems * 4);
        }
        else
        {
            SetScore(numberOfDestroyedGems * 6);
        }

        CheckPointsWithGoal();
    }

    private void CheckPointsWithGoal()
    {
        if(Slider.maxValue == Slider.value)
        {
            GameManager.Round++;
            GameManager.EndRound();
        }
    }

    private void SetRoundGoal(int goalScore)
    {
        Slider.maxValue = goalScore;
        Slider.value = 0;
        SetScoreText(0);
    }

    private void SetScoreText(int newScore)
    {
        ScoreText.text = newScore + "/" + Slider.maxValue;
    }
}
