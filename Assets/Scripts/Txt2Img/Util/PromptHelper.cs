using UnityEngine;

namespace Txt2Img.Util
{
    public static class PromptHelper
    {
        public static Sprite GetPromptResult(PromptTheme promptTheme)
        {
            var aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
            var matchingPrompt = aiManager.PromptResults[promptTheme];
            
            return matchingPrompt.Result;
        }
    }
}