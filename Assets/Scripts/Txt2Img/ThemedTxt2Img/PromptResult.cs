using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class PromptResult : MonoBehaviour
    {
        [FormerlySerializedAs("promptTheme")] [SerializeField]
        public PromptTheme theme;

        [FormerlySerializedAs("promptText")] public string text = "camel";

        [FormerlySerializedAs("promptTextGameObject")]
        public GameObject textGameObject = null;

        [FormerlySerializedAs("promptImageGameObject")]
        public GameObject imageGameObject;

        public GameObject loadingSpinner;
        
        public GameObject downloadPercentage;

        public PromptResult repromptModalResult;
        
        private StableDiffusionText2Image diffusionGenerator;
        
        private TabGroup tabGroup;
        
        private void Start()
        {
            SaveSpriteToAIManager();
            tabGroup = FindObjectOfType<TabGroup>();
            diffusionGenerator = imageGameObject.gameObject.GetComponent<StableDiffusionText2Image>();
        }

        public void OpenRepromptModal()
        {
            AIManager.Instance.editingPromptResult = this;
            repromptModalResult.gameObject.SetActive(true);
        }

        public IEnumerator ProcessReprompting(bool enhancePrompt = true)
        {
            yield return PromptHelper.InvokeTxt2ImgGeneration(this, diffusionGenerator, text, this, enhancePrompt);

            while (diffusionGenerator.generating)
            {
                yield return null;
            }

            yield return true;
        }

        public void SaveSpriteToAIManager()
            => AIManager.Instance.PromptResults[theme] =
                new() { Theme = theme, Text = text, Result = imageGameObject.gameObject.GetComponent<Image>().sprite };

        public void ApplyUIBackgrounds()
        {
            var promptResults = AIManager.Instance.promptResultObjects.ToList();

            promptResults.ForEach(promptResult =>
            {
                var promptThemedObject = promptResult.GetComponent<PromptThemedObject>();
                if (promptThemedObject)
                {
                    promptThemedObject.ApplyFeatures();
                }
            });
        }

        public void ApplyUIButtons()
        {
            var promptThemedObjects = FindObjectsOfType<PromptThemedObject>()
                .Where(obj => obj.promptTheme == PromptTheme.UIButton)
                .ToList();

            promptThemedObjects.ForEach(promptThemedObject => promptThemedObject.ApplyFeatures());
        }

        public void UpdateGenerationProgress(int progress)
        {
            if (progress < 15)
            {
                downloadPercentage.GetComponent<TextMeshProUGUI>().text = 0 + "%";
                return;
            }

            if (!imageGameObject.activeSelf)
            {
                imageGameObject.SetActive(true);
            }

            downloadPercentage.GetComponent<TextMeshProUGUI>().text = progress + "%";
            if (progress > 99)
            {
                downloadPercentage.SetActive(false);
                imageGameObject.SetActive(true);
            }
        }

        public void EnableEditMode()    
        {
            AIManager.Instance.editingPromptResult = this;
            tabGroup.ToGalleryTab();
        }

        public void DisableEditMode()
        {
            AIManager.Instance.editingPromptResult = null;
            SaveSpriteToAIManager();
        }
        
        public void ApplyPromptLabel(string promptText)
        {
            try
            {
                textGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = promptText + "\n" + "(" + theme.ToThemeString() + ")";
                text = promptText;
            }
            catch (UnassignedReferenceException e)
            {
                
            }
        }
    }
}