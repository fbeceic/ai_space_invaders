using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MenuManager : MonoBehaviour
{
    public List<GameObject> menus;

    public static MenuManager Instance;

    public int currentMenu = 0;

    public int menuToShow = 0;

    void Awake()
    {
        var menuObjects = GameObject.FindGameObjectsWithTag("MenuManager");
        var _menuToShow = 0; 
        if (menuObjects.Length > 1)
        {
            foreach (var managerObj in menuObjects)
            {
                var manager = managerObj.GetComponent<MenuManager>();

                if (manager.menus != null && manager.menus.Count > 0)
                {
                    if (Instance != null && Instance != manager)
                    {
                        _menuToShow = Instance.menuToShow;
                        Destroy(Instance.gameObject);
                    }

                    Instance = manager;
                }
                else
                {
                    if (manager != Instance)
                    {
                        Destroy(managerObj);
                    }
                }
            }

            DontDestroyOnLoad(Instance.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        ShowMenu(_menuToShow);
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