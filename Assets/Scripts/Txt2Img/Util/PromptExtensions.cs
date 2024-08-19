using System.Collections.Generic;

namespace Txt2Img.Util
{
    public static class PromptExtensions
    {
        public static readonly Dictionary<PromptTheme, Dictionary<PromptType, List<string>>> Extensions = new()
        {
            {
                PromptTheme.Enemy, new Dictionary<PromptType, List<string>>
                {
                    { PromptType.Main, new List<string> {  "enemy", "90 degrees angle", "centered", "monster"  } },
                    { PromptType.Negative, new List<string> { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity", "man", "human", "person" } }
                }
            },
        };
 
        public static List<string> GetValue(this Dictionary<PromptType, List<string>> dictionary, PromptType key)
            => dictionary.TryGetValue(key, out var value) ? value : new();

        public static Dictionary<PromptType, List<string>> GetValue(
            this Dictionary<PromptTheme, Dictionary<PromptType, List<string>>> dictionary, PromptTheme key)
            => dictionary.TryGetValue(key, out var value) ? value : new();
    }
}
