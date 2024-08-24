using System.Collections.Generic;

namespace Txt2Img.Util
{
    public static class PromptExtensions
    {
        public static readonly Dictionary<PromptTheme, Dictionary<PromptType, List<string>>> Extensions = new()
        {
            {
                PromptTheme.Background, new()
                {
                    { PromptType.Main, new() { "land", "background"} },
                    { PromptType.Negative, new() { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
                }
            },
            {
                PromptTheme.Player, new()
                {
                    { PromptType.Main, new() { "spaceship", "vertical", "top perspective", "90 degrees angle", "centered" } },
                    { PromptType.Negative, new() { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
                }
            },
            {
                PromptTheme.Enemy, new()
                {
                    { PromptType.Main, new() {  "enemy", "90 degrees angle", "centered", "monster"  } },
                    { PromptType.Negative, new() { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity", "man", "human", "person" } }
                }
            },
            {
                PromptTheme.BossEnemy, new()
                {
                    { PromptType.Main, new() {  "enemy", "90 degrees angle", "centered", "monster", "boss", "big"} },
                    { PromptType.Negative, new() { "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity", "man", "human", "person" } }
                }
            },
            {
                PromptTheme.PlayerProjectile, new()
                {
                    { PromptType.Main, new() {  "orb", "circle", "bullet", "projectile", "centered"  } },
                    { PromptType.Negative, new() { "background", "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
                }
            },
            {
                PromptTheme.EnemyProjectile, new()
                {
                    { PromptType.Main, new() {  "orb", "circle", "bullet", "projectile", "centered"  } },
                    { PromptType.Negative, new() { "background", "words", "text", "letters", "realistic", "photograph", "logo", "watermark", "nudity" } }
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
