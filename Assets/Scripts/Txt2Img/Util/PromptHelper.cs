using System;
using System.Collections;
using System.Collections.Generic;
using Txt2Img.ThemedTxt2Img;
using UnityEngine;

namespace Txt2Img.Util
{
    public static class PromptHelper
    {
        public static void InvokeTxt2ImgGeneration(MonoBehaviour monoBehaviour,
            StableDiffusionText2Image diffusionGenerator, string prompt, PromptTheme theme, Action<int> loadingCallback)
        {
            diffusionGenerator.PromptTheme = theme;
            diffusionGenerator.Prompt = ExtendPrompt(prompt, theme, PromptType.Main);
            diffusionGenerator.NegativePrompt = ExtendPrompt("", theme, PromptType.Negative);

            if (!diffusionGenerator.generating)
            {
                monoBehaviour.StartCoroutine(diffusionGenerator.GenerateAsync(loadingCallback));
            }
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