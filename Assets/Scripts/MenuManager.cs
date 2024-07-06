using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class MenuManager : MonoBehaviour
{
    private List<StableDiffusionText2Image> diffusionGenerators;
        
    private List<TMP_InputField> inputFields;
    
    void Start()
    {
        inputFields = new List<TMP_InputField>(FindObjectsOfType<TMP_InputField>());

        SceneManager.LoadScene("Space Invaders", LoadSceneMode.Additive);
        SceneManager.sceneLoaded += DisableObjects;
    }
        
    private void DisableObjects(Scene scene, LoadSceneMode mode)
    {
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
