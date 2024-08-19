using Txt2Img.ThemedTxt2Img;
using UnityEngine;

namespace Txt2Img.Util
{
    public static class PromptHelper
    {
        public static string ExtendPrompt(string prompt, PromptTheme theme, PromptType type)
            => (type == PromptType.Main ? prompt : "") +
               (type == PromptType.Main ? "," : "") +
               string.Join(", ", PromptExtensions.Extensions.GetValue(theme).GetValue(type));

        public static Sprite GetPromptResult(PromptTheme promptTheme)
        {
            var aiManager = AIManager.Instance;
            var matchingPrompt = aiManager.PromptResults[promptTheme];

            return matchingPrompt.Result;
        }

        public static void InvokeTxt2ImgGeneration(StableDiffusionText2Image diffusionGenerator, string prompt, PromptTheme theme)
        {
            diffusionGenerator.PromptTheme = theme;
            diffusionGenerator.Prompt = ExtendPrompt(prompt, theme, PromptType.Main);
            diffusionGenerator.NegativePrompt = ExtendPrompt("", theme, PromptType.Negative);

            if (!diffusionGenerator.generating)
            {
                diffusionGenerator.Generate();
            }

            // Wait for the generation to complete
            while (diffusionGenerator.generating)
            {
                // loading
            }
        }
    }
}
