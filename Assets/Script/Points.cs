using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
            SetScore(minimumMatchNumber);
        }
        else if (numberOfDestroyedGems > minimumMatchNumber && numberOfDestroyedGems <= 2 * minimumMatchNumber)
        {
            SetScore(numberOfDestroyedGems * 2);
        }
        else
        {
            SetScore(numberOfDestroyedGems * 3);
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
