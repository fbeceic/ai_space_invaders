using Txt2Img.Util;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class PromptResult : MonoBehaviour
    {
        [FormerlySerializedAs("promptTheme")]
        [SerializeField] public PromptTheme theme;

        [FormerlySerializedAs("promptText")]
        public string text = "camel";

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

            PromptHelper.InvokeTxt2ImgGeneration(diffusionGenerator, text, theme);

            SaveSpriteToAIManager();

            loadingSpinner.SetActive(false);
        }

        public void SaveSpriteToAIManager()
            => AIManager.Instance.PromptResults[theme] =
                new() { Theme = theme, Text = text, Result = imageGameObject.gameObject.GetComponent<Image>().sprite };
    }
}
