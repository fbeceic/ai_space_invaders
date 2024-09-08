using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MenuManager : MonoBehaviour
{
    public List<GameObject> menus;

    public static MenuManager Instance;

    public int currentMenu = 0;

    public int menuToShow = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ShowMenu(Instance.menuToShow);
    }

    public void ShowMenu(int index)
    {
        foreach (GameObject menu in menus)
        {
            menu.SetActive(false);
        }

        if (index >= 0 && index < menus.Count)
        {
            menus[index].SetActive(true);
            currentMenu = index;
        }
    }

    public void RunGame()
    {
        SceneManager.LoadScene("Space Invaders");
    }
}