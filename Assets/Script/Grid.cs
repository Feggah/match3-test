using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public int rows, columns;
    private Gem[,] gridGems;
    private GameObject[] gems;

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
                gridGems[x, y] = InstantiateGems(x, y);
            }
        }
    }

    private Gem InstantiateGems(int x, int y)
    {
        GameObject randomGem = gems[UnityEngine.Random.Range(0, gems.Length)];
        Gem newGem = Instantiate(randomGem, new Vector2(x, y), Quaternion.identity).GetComponent<Gem>();
        newGem.ChangeGemPosition(x, y);
        return newGem;
    }

    private void LoadGems()
    {
        gems = Resources.LoadAll<GameObject>("Prefabs");
        for (int i = 0; i < gems.Length; i++)
        {
            gems[i].GetComponent<Gem>().gemType = (GemType)i;
        }
    }
}
