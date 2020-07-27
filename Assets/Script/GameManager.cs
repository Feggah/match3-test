using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [HideInInspector]
    public int Round;

    [HideInInspector]
    public bool ContinueRoundProcess = true;

    public bool CanCount = true;

    [SerializeField]
    private GridController GridController;

    [SerializeField]
    private Grid Grid;

    [SerializeField]
    private Points Points;

    [SerializeField]
    private SceneController SceneController;

    [SerializeField]
    private Text RoundText;

    [SerializeField]
    private Text TimerText;

    private double secondsTimer = 0f;

    private int minutesTimer = 2;

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
        GridController.DetectTouches = false;
        SaveRound();
        SceneController.ChangeScene("Game");
    }

    public void KillCoroutines()
    {
        GridController.Kill();
    }

    public IEnumerator PanelPopUp(string name)
    {
        yield return StartCoroutine(SceneController.PopUp(name));
    }

    public void ButtonUIPopUp(string name)
    {
        StartCoroutine(SceneController.PopUp(name));
    }

    private void UpdateTimer()
    {
        if (CanCount)
        {
            if (secondsTimer < 0f)
            {
                minutesTimer--;
                if (minutesTimer < 0)
                {
                    SetTimerText(0, 0);
                    ContinueRoundProcess = false;
                    KillCoroutines();
                    StartCoroutine(PanelPopUp("Failed"));
                }
                else
                {
                    secondsTimer = 60f;
                    SetTimerText(minutesTimer, (int)secondsTimer);
                }
            }

            else if (secondsTimer >= 0f)
            {
                secondsTimer -= Time.deltaTime / 2.5;
                SetTimerText(minutesTimer, (int)secondsTimer);
            }
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
