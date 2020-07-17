using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemType GemType { get; private set; }

    public Vector2 Position { get; private set; }

    public void ChangeGemPosition(float newX, float newY)
    {
        Position = new Vector2(newX, newY);
    }
}
