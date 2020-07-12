using UnityEngine;

public class PossibleGem : MonoBehaviour
{
    public GemType GemType { get; private set; }

    public void SetGemType(GemType gemType)
    {
        GemType = gemType;
        gameObject.name = GemType.ToString();
    }
}
