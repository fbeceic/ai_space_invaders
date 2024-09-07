using System;

namespace Txt2Img.Util
{
    public enum PromptTheme
    {
        Background,
        Player,
        Enemy,
        BossEnemy,
        PlayerProjectile,
        EnemyProjectile,

        UIBackground,
        UIButton
    }

    public static class PromptThemeHelpers
    {
        public static string ToThemeString(this PromptTheme theme)
        {
            return theme switch
            {
                PromptTheme.Background => "Background",
                PromptTheme.Player => "Player",
                PromptTheme.Enemy => "Enemy",
                PromptTheme.BossEnemy => "Boss Enemy",
                PromptTheme.PlayerProjectile => "Player Projectile",
                PromptTheme.EnemyProjectile => "Enemy Projectile",
                PromptTheme.UIBackground => "UI Background",
                PromptTheme.UIButton => "UI Button",
                _ => theme.ToString()
            };
        }

        public static string ToPromptThemeString(this string filenameThemePart)
        {
            return filenameThemePart.ToLower() switch
            {
                "background" => ToThemeString(PromptTheme.Background),
                "player" => ToThemeString(PromptTheme.Player),
                "enemy" => ToThemeString(PromptTheme.Enemy),
                "bossenemy" => ToThemeString(PromptTheme.BossEnemy),
                "playerprojectile" => ToThemeString(PromptTheme.PlayerProjectile),
                "enemyprojectile" => ToThemeString(PromptTheme.EnemyProjectile),
                "uibackground" => ToThemeString(PromptTheme.UIBackground),
                "uibutton" => ToThemeString(PromptTheme.UIButton),
                _ => throw new ArgumentException("Invalid theme string", nameof(filenameThemePart))
            };
        }

        public static PromptTheme ToPromptTheme(this string filenameThemePart)
        {
            return filenameThemePart.ToLower() switch
            {
                "background" => (PromptTheme.Background),
                "player" => (PromptTheme.Player),
                "enemy" => (PromptTheme.Enemy),
                "bossenemy" => (PromptTheme.BossEnemy),
                "playerprojectile" => (PromptTheme.PlayerProjectile),
                "enemyprojectile" => (PromptTheme.EnemyProjectile),
                "uibackground" => (PromptTheme.UIBackground),
                "uibutton" => (PromptTheme.UIButton),
                _ => throw new ArgumentException("Invalid theme string", nameof(filenameThemePart))
            };
        }
    }
}