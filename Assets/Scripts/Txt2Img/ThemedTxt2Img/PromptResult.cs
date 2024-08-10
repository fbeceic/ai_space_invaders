using System;
using System.Threading.Tasks;
using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class PromptResult : MonoBehaviour
    {
        [SerializeField] public PromptTheme promptTheme;

        public GameObject promptTextGameObject;

        public GameObject promptImageGameObject;

        public GameObject loadingSpinner;

        private string promptText = "camel";

        public void ApplyPromptFeatures(string text, Sprite sprite)
        {
            promptImageGameObject.gameObject.GetComponent<Image>().sprite = sprite;
            promptTextGameObject.gameObject.GetComponent<Text>().text = text + "\n" + "(" +
                                                                        promptTheme.ToString()
                                                                            .Replace(
                                                                                "(?<!^)(?<!\\s)(?<![a-z])(?=[A-Z])",
                                                                                " ") + ")";
            promptText = text;
        }

        private void Start()
        {
            SaveSpriteToAIManager();
        }

        public void RepromptResult()
        {
            var diffusionGenerator = promptImageGameObject.gameObject.GetComponent<StableDiffusionText2Image>();
            diffusionGenerator.PromptTheme = promptTheme;
            diffusionGenerator.Prompt = PromptHelper.ExtendPrompt(promptText, promptTheme, PromptType.Main);
            diffusionGenerator.NegativePrompt = PromptHelper.ExtendPrompt("", promptTheme, PromptType.Negative);

            if (!diffusionGenerator.generating)
            {
                StartCoroutine(diffusionGenerator.GenerateAsync(ShowLoading));
            }

            // Wait for the generation to complete
            while (diffusionGenerator.generating)
            {
                if (!loadingSpinner.activeSelf)
                {
                    loadingSpinner.SetActive(true);
                }
                // You can add progress indication here if needed
            }

            SaveSpriteToAIManager();

            loadingSpinner.SetActive(false);
        }

        private void ShowLoading(int percentage)
        {
            if (!loadingSpinner.activeSelf)
            {
                loadingSpinner.SetActive(true);
            }
        }

        public void SaveSpriteToAIManager()
        {
            var aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
            var result = promptImageGameObject.gameObject.GetComponent<Image>().sprite;

            aiManager.PromptResults[promptTheme] =
                new Prompt { Theme = promptTheme, Text = promptText, Result = result };
        }
    }
}