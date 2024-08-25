using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MenuManager : MonoBehaviour
{
    public List<GameObject> menus;

    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        //ShowMenu(0); 
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
        }
    }

    public void RunGame()
    {
        SceneManager.LoadScene("Space Invaders");
    }
}