using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemType GemType { get; private set; }

    public Vector2 Position { get; private set; }

    public void ChangeGemPosition(int newX, int newY)
    {
        Position = new Vector2(newX, newY);
        gameObject.name = string.Format("[{0}] in [{1}]", GemType, Position);
    }
}
