using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
    public int Rows;

    public int Columns;

    public GridController GridController;

    public GridGem[,] GridGems;

    void Start()
    {
        GridController.LoadGems();
        GridController.CreateGrid();
        GridController.ClearInitialMatches();
    }

    void Update()
    {
        GridController.DetectInputEvents();
    }
}
