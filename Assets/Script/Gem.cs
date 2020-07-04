using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemType gemType;

    public Vector2 position
    {
        get;
        private set;
    }

    public void ChangeGemPosition(int newX, int newY)
    {
        position = new Vector2(newX, newY);
        gameObject.name = string.Format("[{0}] in [{1}]", gemType, position);
    }
}
