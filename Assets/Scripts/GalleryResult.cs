using System;
using TMPro;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

public class GalleryResult : MonoBehaviour
{
    public Image image;

    public TextMeshProUGUI resultName;

    private string resultPrompt;

    private TabGroup tabGroup;

    private void Start()
    {
        tabGroup = FindObjectOfType<TabGroup>();
    }

    public void ApplyResultFeatures(string filename, Sprite sprite)
    {
        var filenameWithoutExtension = filename.Substring(0, filename.LastIndexOf(".", StringComparison.Ordinal));
        var prompt = filenameWithoutExtension.Split("_")[1];
        var filenameThemePart = filenameWithoutExtension.Split("_")[0];
        
        resultName.text = prompt + "\n" + "(" + filenameThemePart.ToPromptTheme() + ")";
        resultPrompt = prompt;
        image.sprite = sprite;
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
        editingPromptResult.ApplyPromptLabel(resultPrompt);  
    }
}