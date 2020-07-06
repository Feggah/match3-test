using UnityEngine;

public class Grid : MonoBehaviour
{
    public int rows;

    public int columns;

    private Gem[,] gridGems;

    private PossibleGem[] gems;

    void Start()
    {
        LoadGems();
        CreateGrid();
    }

    private void CreateGrid()
    {
        gridGems = new Gem[columns, rows];
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridGems[x, y] = InstantiateGem(x, y);
            }
        }
    }

    private Gem InstantiateGem(int x, int y)
    {
        PossibleGem randomGem = gems[UnityEngine.Random.Range(0, gems.Length)];
        Gem newGem = Instantiate(randomGem, new Vector2(x, y), Quaternion.identity).GetComponent<Gem>();
        newGem.ChangeGemPosition(x, y);
        return newGem;
    }

    private void LoadGems()
    {
        gems = Resources.LoadAll<PossibleGem>("Prefabs");
        for (int i = 0; i < gems.Length; i++)
        {
            gems[i].SetGemType((GemType)i);
        }
    }
}
