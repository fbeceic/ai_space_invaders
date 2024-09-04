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
        public GameObject textGameObject;

        [FormerlySerializedAs("promptImageGameObject")]
        public GameObject imageGameObject;

        public GameObject loadingSpinner;
        
        public GameObject downloadPercentage;

        private StableDiffusionText2Image diffusionGenerator;
        
        private TabGroup tabGroup;
        
        private void Start()
        {
            SaveSpriteToAIManager();
            tabGroup = FindObjectOfType<TabGroup>();
            diffusionGenerator = imageGameObject.gameObject.GetComponent<StableDiffusionText2Image>();
        }

        public void ApplyPromptLabel(string promptText)
        {
            textGameObject.gameObject.GetComponent<TextMeshProUGUI>().text = promptText + "\n" + "(" + theme.ToThemeString() + ")";
            text = promptText;
        }

        public void RepromptResult()
        {
            ApplyPromptLabel(text);
            StartCoroutine(ProcessReprompting());
        }

        private IEnumerator ProcessReprompting()
        {
            yield return PromptHelper.InvokeTxt2ImgGeneration(this, diffusionGenerator, text, this);

            while (diffusionGenerator.generating)
            {
                yield return null;
            }
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
            AIManager.Instance.EditingPromptResult = this;
            tabGroup.ToGalleryTab();
        }

        public void DisableEditMode()
        {
            AIManager.Instance.EditingPromptResult = null;
            SaveSpriteToAIManager();
        }
    }
}