using System.Collections.Generic;

namespace Txt2Img.Util
{
    public static class PromptExtensions
    {
        public static readonly Dictionary<PromptTheme, Dictionary<PromptType, List<string>>> Extensions = new()
        {
            {
                PromptTheme.Background, new Dictionary<PromptType, List<string>>
                {
                    { PromptType.Main, new List<string> { "land", "background"} },
                    { PromptType.Negative, new List<string> { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
                }
            },
            {
                PromptTheme.Player, new Dictionary<PromptType, List<string>>
                {
                    { PromptType.Main, new List<string> { "spaceship", "vertical", "top perspective", "90 degrees angle", "centered" } },
                    { PromptType.Negative, new List<string> { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
                }
            },
            {
                PromptTheme.Enemy, new Dictionary<PromptType, List<string>>
                {
                    { PromptType.Main, new List<string> {  "enemy", "90 degrees angle", "centered", "monster"  } },
                    { PromptType.Negative, new List<string> { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity", "man", "human", "person" } }
                }
            },
            {
                PromptTheme.Projectile, new Dictionary<PromptType, List<string>>
                {
                    { PromptType.Main, new List<string> {  "orb", "circle", "bullet", "projectile", "centered"  } },
                    { PromptType.Negative, new List<string> { "background", "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
                }
            },
        };

        public static List<string> GetValue(this Dictionary<PromptType, List<string>> dictionary, PromptType key)
            => dictionary.TryGetValue(key, out var value) ? value : new List<string>();

        public static Dictionary<PromptType, List<string>> GetValue(
            this Dictionary<PromptTheme, Dictionary<PromptType, List<string>>> dictionary, PromptTheme key)
            => dictionary.TryGetValue(key, out var value) ? value : new Dictionary<PromptType, List<string>>();
    }
}