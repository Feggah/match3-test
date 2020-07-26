using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GridController GridController;

    [SerializeField]
    private Grid Grid;

    [SerializeField]
    private Points Points;

    [SerializeField]
    private SceneController scene;

    [SerializeField]
    private Text RoundText;

    [SerializeField]
    private Text TimerText;

    private double secondsTimer = 0f;
    private int minutesTimer = 2;

    [HideInInspector]
    public int Round;

    void Start()
    {
        StartRound();
    }

    void Update()
    {
        UpdateTimer();
    }

    public void UpdateScore(int destroyedGems)
    {
        Points.ChangeScore(destroyedGems, GridController.MinimumMatchNumber);
    }

    public void SaveRound()
    {
        SaveSystem.SaveGame(this);
    }

    public void EndRound()
    {
        SaveRound();
        scene.ChangeScene("Game");
    }

    private void UpdateTimer()
    {
        if(secondsTimer < 0f)
        {
            minutesTimer--;
            if (minutesTimer < 0)
            {
                SetTimerText(0, 0);
                EndRound();
            }
            else
            {
                secondsTimer = 60f;
                SetTimerText(minutesTimer, (int)secondsTimer);
            }
        }

        else if(secondsTimer >= 0f)
        {
            secondsTimer -= Time.deltaTime/2.5;
            SetTimerText(minutesTimer, (int)secondsTimer);
        }
    }

    private void SetTimerText(int minutes, int seconds)
    {
        TimerText.text = minutes + ":" + seconds.ToString("D2");
    }

    private void StartRound()
    {
        LoadRound();
        Points.CalculateGoalScore(Round);
        StartCoroutine(Grid.StartRound());
    }

    private void LoadRound()
    {
        ProgressData data = SaveSystem.LoadGame();

        Round = (data == null) ? 1 : data.Round;

        SetRoundText(Round);
    }

    private void SetRoundText(int round)
    {
        RoundText.text = round.ToString();
    }
}
