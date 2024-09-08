using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using JetBrains.Annotations;
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

        [FormerlySerializedAs("promptText")] public string text = "";

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
            //SaveSpriteToAIManager();
            tabGroup = FindObjectOfType<TabGroup>();
            diffusionGenerator = imageGameObject.gameObject.GetComponent<StableDiffusionText2Image>();
            if (AIManager.Instance.editingPromptResult == null)
            {
                LoadSpriteFromAIManager();
            }
        }

        void LoadSpriteFromAIManager()
        {
            var savedPromptResult = AIManager.Instance.PromptResults;
            if (savedPromptResult[theme] != null)
            {
                var savedPromptImage = savedPromptResult[theme].Result;
                var savedPromptText = savedPromptResult[theme].Text;
                imageGameObject.GetComponent<Image>().sprite = savedPromptImage;
                ApplyPromptLabel(savedPromptText.Length > 0 ? savedPromptText : "DEFAULT ELEMENT");
            }
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

        public void SaveSpriteToAIManager([CanBeNull] string prompt = null)
            => AIManager.Instance.PromptResults[theme] =
                new() { Theme = theme, Text = prompt ?? text, Result = imageGameObject.gameObject.GetComponent<Image>().sprite };

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
            tabGroup.ToGalleryTab(tabGroup.selectedTab);
        }

        public void DisableEditMode()
        {
            AIManager.Instance.editingPromptResult = null;
        }

        public void ApplyPromptLabel(string promptText)
        {
            try
            {
                textGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = promptText + "\n" + "(" + theme.ToThemeString() + ")";
                text = promptText;
                AIManager.Instance.PromptResults[theme].Text = promptText;
            }
            catch (UnassignedReferenceException)
            {
            }
        }
    }
}