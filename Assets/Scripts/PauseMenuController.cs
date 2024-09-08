using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public Button storyMenuButton;
    public Button elementsMenuButton;
    public Button exitButton;

    void Start()
    {
        storyMenuButton.onClick.AddListener(OnStoryMenuButtonClick);
        elementsMenuButton.onClick.AddListener(OnElementsMenuButtonClick);
        exitButton.onClick.AddListener(OnExitButtonClick);

    }

    void OnStoryMenuButtonClick()
    {
        MenuManager.Instance.menuToShow = 0;
        SceneManager.LoadScene("Prompt Menu");
    }

    void OnElementsMenuButtonClick()
    {
        MenuManager.Instance.menuToShow = 1;
        SceneManager.LoadScene("Prompt Menu");
    }

    void OnExitButtonClick()
    {
        Application.Quit();
    }
}
