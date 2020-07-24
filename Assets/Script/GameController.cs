using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private GridController GridController;

    [SerializeField]
    private Points Points;

    [SerializeField]
    private int Round;

    void Start()
    {
        Points.CalculateGoalScore(Round);
    }

    public void UpdateScore(int destroyedGems)
    {
        Points.ChangeScore(destroyedGems, GridController.MinimumMatchNumber);
    }
}
