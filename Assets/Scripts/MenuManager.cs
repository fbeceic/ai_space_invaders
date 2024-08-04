using System.Collections.Generic;
using System.Linq;
using TMPro;
using Txt2Img.ThemedTxt2Img;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public sealed class MenuManager : MonoBehaviour
{
    public List<GameObject> menus;
    
    private List<StableDiffusionText2Image> diffusionGenerators;

    private List<TMP_InputField> inputFields;
    
    public static MenuManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        inputFields = new List<TMP_InputField>(FindObjectsOfType<TMP_InputField>());
        
        ShowMenu(0); 
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

    private void DisableObjects(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive)
        {
            return;
        }
        
        diffusionGenerators = new List<StableDiffusionText2Image>(FindObjectsOfType<StableDiffusionText2Image>());
        GameObject[] objectsInScene = scene.GetRootGameObjects();

        foreach (var obj in objectsInScene)
        {
            if (obj.GetComponent<StableDiffusionText2Image>() == null)
            {
                obj.SetActive(false);
            }
        }
    }
}