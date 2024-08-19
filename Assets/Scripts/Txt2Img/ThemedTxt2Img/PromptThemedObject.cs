using Txt2Img.Util;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Txt2Img.ThemedTxt2Img
{
    public class PromptThemedObject : MonoBehaviour
    {
        [SerializeField] public PromptTheme promptTheme;

        private void Start()
        {
            if (SceneManager.GetActiveScene().name == "Prompt Menu") return;
            
            ApplyPromptResult();
            ApplyScale();
            if (promptTheme != PromptTheme.Background)
            {
                ReinitializeCollider();
            }
        }

        private void ApplyPromptResult()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var image = GetComponent<Image>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = CreateSpriteWithOffset();
            }
            else if (image != null)
            {
                image.sprite = CreateSpriteWithOffset();
            }
        }

        private void ApplyScale()
        {
            var (x, y) = GetTransformScale();
            transform.localScale = new Vector3() { x = x, y = y };
        }
        
        private void ReinitializeCollider()
        {
            var currentPolygonCollider = GetComponent<PolygonCollider2D>();
            if (currentPolygonCollider == null)
            {
                return;
            }

            Destroy(currentPolygonCollider);

            var newPolygonCollider = gameObject.AddComponent<PolygonCollider2D>();
            newPolygonCollider.isTrigger = true;
        }

        private Sprite CreateSpriteWithOffset()
        {
            var spriteTexture = PromptHelper.GetPromptResult(promptTheme).texture;
            var (width, height) = GetSpriteSize();
            var spriteRect = new Rect(0.0f, 0.0f, width, height);

            return Sprite.Create(spriteTexture, spriteRect, new Vector2 { x = 0.5f, y = 0.5f });
        }

        private (int width, int height) GetSpriteSize()
        {
            switch (promptTheme)
            {
                case PromptTheme.Background:
                    return (Constants.GeneratedBackgroundWidth, Constants.GeneratedBackgroundHeight);
                case PromptTheme.PlayerProjectile:
                case PromptTheme.Enemy:
                case PromptTheme.Player:
                case PromptTheme.BossEnemy:
                case PromptTheme.EnemyProjectile:
                default:
                    return (Constants.GeneratedSpriteWidth, Constants.GeneratedSpriteHeight);
            }
        }
        
        private (float x, float y) GetTransformScale()
        {
            switch (promptTheme)
            {
                case PromptTheme.PlayerProjectile:
                case PromptTheme.EnemyProjectile:
                    return (0.2f, 0.2f);
                case PromptTheme.Background:
                    return (5.5f, 5.5f);
                case PromptTheme.Enemy:
                    return (0.7f, 0.7f);
                case PromptTheme.BossEnemy:
                    return (1.1f, 1.1f);
                case PromptTheme.Player:
                default:
                    return (1.0f, 1.0f);
            }
        }
    }
}
