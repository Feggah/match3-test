[System.Serializable]
public class ProgressData
{
    public int Round;

    public ProgressData (GameManager gameInfo)
    {
        Round = gameInfo.Round;
    }
}
