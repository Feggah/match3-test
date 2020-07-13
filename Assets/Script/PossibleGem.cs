using UnityEngine;

public class PossibleGem : Gem
{
    public void SetGemType(GemType gemType)
    {
        GemType = gemType;
        gameObject.name = GemType.ToString();
    }
}
