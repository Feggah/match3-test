using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneController : MonoBehaviour
{
    [SerializeField]
    private GameObject ConfirmPanel;

    [SerializeField]
    public GameObject CompletedPanel;

    [SerializeField]
    public GameObject FailedPanel;

    [SerializeField]
    private GameObject ShufflePanel;

    [SerializeField]
    private GridController GridController;

    [SerializeField]
    private GameManager GameManager;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitScene()
    {
        FindObjectOfType<AudioManager>().Play("SelectSound");
        Application.Quit();
    }

    public void SelectedPanelOption(bool exit)
    {
        if (exit)
        {
            ChangeScene("Menu");
        }
        ConfirmPanel.SetActive(false);
        FindObjectOfType<AudioManager>().Play("SelectSound");
        GridController.DetectTouches = true;
        GameManager.CanCount = true;
    }

    public IEnumerator PopUp(string name)
    {
        switch (name)
        {
            case "Completed":
                GridController.DetectTouches = false;
                CompletedPanel.SetActive(true);

                yield return new WaitForSeconds(4f);

                CompletedPanel.SetActive(false);
                GameManager.EndRound();
                break;

            case "Failed":
                GridController.DetectTouches = false;
                GameManager.CanCount = false;
                FailedPanel.SetActive(true);

                yield return new WaitForSeconds(4f);

                FailedPanel.SetActive(false);
                GameManager.CanCount = true;
                GameManager.EndRound();
                break;

            case "Shuffle":
                if (!ShufflePanel.activeSelf)
                {
                    ShufflePanel.SetActive(true);
                }
                else
                {
                    ShufflePanel.SetActive(false);
                }
                break;

            case "Back":
                if (GridController.DetectTouches)
                {
                    FindObjectOfType<AudioManager>().Play("SelectSound");
                    GridController.DetectTouches = false;
                    GameManager.CanCount = false;
                    if (ConfirmPanel)
                    {
                        ConfirmPanel.SetActive(true);
                    }
                }
                break;
        }
    }
}
