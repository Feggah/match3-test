using UnityEngine;

public class Grid : MonoBehaviour
{
    public int Rows;

    public int Columns;

    public GridController GridController;

    public Gem[,] GridGems;

    void Start()
    {
        GridController.LoadGems();
        CreateGrid();
    }

    void Update()
    {
        GridController.DetectInputEvents();
    }

    public void CreateGrid()
    {
        GridGems = new Gem[Columns, Rows];

        for (int x = 0; x < Columns; x++)
        {
            for (int y = 0; y < Rows; y++)
            {
                GridGems[x, y] = GridController.InstantiateGem(x, y);
            }
        }
    }
}
