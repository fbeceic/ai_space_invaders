using System;
using UnityEngine;
using UnityEngine.UI;

public class GalleryResult : MonoBehaviour
{
    public Image image;

    private TabGroup tabGroup;

    private void Start()
    {
        tabGroup = FindObjectOfType<TabGroup>();
    }

    public void SwitchPromptImage()
    {
        if (!AIManager.Instance.EditingPromptResult)
        {
            return;
        }

        var editingPromptResult = AIManager.Instance.EditingPromptResult;
        var imageToChange = editingPromptResult.imageGameObject.gameObject.GetComponent<Image>().sprite;
        editingPromptResult.imageGameObject.gameObject.GetComponent<Image>().sprite = image.sprite;

        AIManager.Instance.PromptResults[editingPromptResult.theme] =
            new() { Theme = editingPromptResult.theme, Text = editingPromptResult.text, Result = imageToChange };

        tabGroup.ToGameElementsTab();
        editingPromptResult.DisableEditMode();  
    }
}