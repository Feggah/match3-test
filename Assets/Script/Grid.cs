using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public int Rows;

    public int Columns;

    public GridGem[,] GridGems;

    [SerializeField]
    private GridController GridController;

    public IEnumerator StartRound()
    {
        GridController.LoadGems();
        GridController.CreateGrid();
        GridController.ClearInitialMatches();
        yield return StartCoroutine(GridController.CheckPossibleMoves());
    }

    void Update()
    {
        GridController.DetectInputEvents();
    }
}
