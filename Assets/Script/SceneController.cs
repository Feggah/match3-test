using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private GameObject ConfirmPanel;

    [SerializeField]
    private GridController GridController;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitScene()
    {
        FindObjectOfType<AudioManager>().Play("SelectSound");
        Application.Quit();
    }

    public void ExitPopUp()
    {
        if (GridController.DetectTouches)
        {
            FindObjectOfType<AudioManager>().Play("SelectSound");
            GridController.DetectTouches = false;
            if (ConfirmPanel)
            {
                ConfirmPanel.SetActive(true);
            }
        }
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
    }
}
