using System;
using System.Collections;
using System.Collections.Generic;
using Txt2Img.ThemedTxt2Img;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img.Util
{
    public static class PromptHelper
    {
        public static IEnumerator InvokeTxt2ImgGeneration(MonoBehaviour monoBehaviour,
            StableDiffusionText2Image diffusionGenerator, string prompt, PromptResult promptResult, bool enhancePrompt = true)
        {
            AIManager.Instance.isDiffusionInProgress = true;
            AIManager.Instance._audioSource.PlayOneShot(AIManager.Instance.spriteGenerateBeginSound);
                
            promptResult.imageGameObject.GetComponent<Image>().sprite = null;
            promptResult.downloadPercentage.SetActive(true);
            promptResult.loadingSpinner.SetActive(true);

            diffusionGenerator.PromptTheme = promptResult.theme;
            diffusionGenerator.Prompt = enhancePrompt ? ExtendPrompt(prompt, promptResult.theme, PromptType.Main) : prompt;
            diffusionGenerator.NegativePrompt = ExtendPrompt("", promptResult.theme, PromptType.Negative);

            if (!diffusionGenerator.generating)
            {
                promptResult.imageGameObject.SetActive(false);
                monoBehaviour.StartCoroutine(diffusionGenerator.GenerateAsync(promptResult.UpdateGenerationProgress));
            }

            while (diffusionGenerator.generating)
            {
                yield return null;
            }
            
            promptResult.ApplyPromptLabel(prompt);
            promptResult.SaveSpriteToAIManager();

            if (promptResult.theme is PromptTheme.UIBackground)
            {
                promptResult.ApplyUIBackgrounds();
            }

            if (promptResult.theme is PromptTheme.UIButton)
            {
                promptResult.ApplyUIButtons();
            }
            
            promptResult.downloadPercentage.SetActive(false);
            promptResult.loadingSpinner.SetActive(false);

            AIManager.Instance._audioSource.PlayOneShot(AIManager.Instance.spriteGenerateEndSound);
            AIManager.Instance.isDiffusionInProgress = false;
        }

        private static string ExtendPrompt(string prompt, PromptTheme theme, PromptType type)
            => (type == PromptType.Main ? prompt : "") +
               (type == PromptType.Main ? ", " : "") +
               string.Join(", ", PromptExtensions.Extensions.GetValue(theme).GetValue(type));

        public static Sprite GetPromptResult(PromptTheme promptTheme)
        {
            var aiManager = AIManager.Instance;
            var matchingPrompt = aiManager.PromptResults[promptTheme];

            return matchingPrompt.Result;
        }
    }
}