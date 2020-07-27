using System.Collections;
using UnityEngine;

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
        yield return new WaitForSeconds(2f);
        yield return StartCoroutine(GridController.CheckPossibleMoves());
        GridController.DetectTouches = true;
    }

    void Update()
    {
        GridController.DetectInputEvents();
    }
}
