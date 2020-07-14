using UnityEngine;

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
    }

    void Update()
    {
        GridController.DetectInputEvents();
    }
}
