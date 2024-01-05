using UnityEngine;
using UnityEngine.UI;

public class ControlMenu : MonoBehaviour
{
    public Button backButton;
    public Button exitButton;

    void Start()
    {
        backButton.onClick.AddListener(BackButtonClicked);
        exitButton.onClick.AddListener(ExitButtonClicked);
    }

    void BackButtonClicked()
    {
        gameObject.SetActive(false);
    }

    void ExitButtonClicked()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
