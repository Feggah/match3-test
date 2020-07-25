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

    void Start()
    {
        GridController.LoadGems();
        GridController.CreateGrid();
        GridController.ClearInitialMatches();
        GridController.CheckPossibleMoves();
    }

    void Update()
    {
        GridController.DetectInputEvents();
    }
}
