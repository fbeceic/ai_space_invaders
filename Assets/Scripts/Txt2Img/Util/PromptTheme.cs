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

    public static class PromptThemeToString
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
    }
}