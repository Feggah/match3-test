using UnityEngine;

public class Gem : MonoBehaviour
{
    public GemType GemType { get; protected set; }

    public void SetGemType(GemType gemType)
    {
        GemType = gemType;
        gameObject.name = GemType.ToString();
    }
}
