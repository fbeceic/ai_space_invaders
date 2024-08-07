using UnityEngine;

namespace Txt2Img.Util
{
    public static class PromptHelper
    {
        public static string ExtendPrompt(string prompt, PromptTheme theme, PromptType type)
            => (type == PromptType.Main ? prompt : "") + (type == PromptType.Main ? "," : "") +
               string.Join(", ", PromptExtensions.Extensions.GetValue(theme).GetValue(type));

        public static Sprite GetPromptResult(PromptTheme promptTheme)
        {
            var aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
            var matchingPrompt = aiManager.PromptResults[promptTheme];
            
            return matchingPrompt.Result;
        }
    }
}