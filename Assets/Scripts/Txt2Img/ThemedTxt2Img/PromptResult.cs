using System.Collections;
using System.Collections.Generic;
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

        public void ApplyPromptFeatures(string promptText, Sprite sprite)
        {
            imageGameObject.gameObject.GetComponent<Image>().sprite = sprite;
            textGameObject.gameObject.GetComponent<Text>().text = promptText +
                                                                  "\n" +
                                                                  "(" +
                                                                  theme.ToString()
                                                                      .Replace(
                                                                          "(?<!^)(?<!\\s)(?<![a-z])(?=[A-Z])",
                                                                          " ") +
                                                                  ")";
            text = promptText;
        }

        private void Start()
        {
            SaveSpriteToAIManager();
        }

        public void RepromptResult()
        {
            var diffusionGenerator = imageGameObject.gameObject.GetComponent<StableDiffusionText2Image>();
            
            loadingSpinner.SetActive(true);
            imageGameObject.SetActive(false);
            
            StartCoroutine(ProcessReprompting(diffusionGenerator));
            
        }
        
        private IEnumerator ProcessReprompting(StableDiffusionText2Image diffusionGenerator)
        {
            PromptHelper.InvokeTxt2ImgGeneration(this, diffusionGenerator, text, theme, UpdateProgressBar);
            
            while (diffusionGenerator.generating)
            {
                yield return null;
            }

            SaveSpriteToAIManager();

            loadingSpinner.SetActive(false);
            imageGameObject.SetActive(true);
        }

        public void SaveSpriteToAIManager()
            => AIManager.Instance.PromptResults[theme] =
                new() { Theme = theme, Text = text, Result = imageGameObject.gameObject.GetComponent<Image>().sprite };

        public void UpdateProgressBar(int progress)
        {
            loadingSpinner.SetActive(true);
        }
    }
}