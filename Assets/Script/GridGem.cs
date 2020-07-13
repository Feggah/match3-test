using UnityEngine;

public class GridGem : Gem
{
    public Vector2 Position { get; private set; }

    public void ChangeGemPosition(float newX, float newY)
    {
        Position = new Vector2(newX, newY);
    }
}
