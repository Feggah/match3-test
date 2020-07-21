using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GridGem : Gem
{
    public Vector2Int Position { get; private set; }

    public void ChangeGemPosition(int newX, int newY)
    {
        Position = new Vector2Int(newX, newY);
    }
}
