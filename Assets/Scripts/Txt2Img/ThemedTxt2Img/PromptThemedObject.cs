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
            ApplyFeatures();
        }

        public void ApplyFeatures()
        {
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
                spriteRenderer.sprite = CreateSpriteFromPromptResult();
            }
            else if (image != null)
            {
                image.sprite = CreateSpriteFromPromptResult();
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

        private Sprite CreateSpriteFromPromptResult()
        {
            var spriteTexture = PromptHelper.GetPromptResult(promptTheme).texture;
            var (width, height) = GetSpriteSize();
            var spriteRect = new Rect(0.0f, 0.0f, width, height);

            return Sprite.Create(spriteTexture, spriteRect, new Vector2 { x = 0.5f, y = 0.5f });
        }

        private (int width, int height) GetSpriteSize() =>
            promptTheme switch
            {
                PromptTheme.Background => (Constants.GeneratedBackgroundWidth, Constants.GeneratedBackgroundHeight),
                PromptTheme.UIButton => (Constants.GeneratedUiButtonWidth, Constants.GeneratedUiButtonHeight),
                PromptTheme.PlayerProjectile or
                    PromptTheme.Enemy or
                    PromptTheme.Player or
                    PromptTheme.BossEnemy or
                    PromptTheme.EnemyProjectile or
                    PromptTheme.UIBackground => (Constants.GeneratedSpriteWidth, Constants.GeneratedSpriteHeight),
                _ => (Constants.GeneratedSpriteWidth, Constants.GeneratedSpriteHeight)
            };

        private (float x, float y) GetTransformScale() =>
            promptTheme switch
            {
                PromptTheme.PlayerProjectile or PromptTheme.EnemyProjectile => (0.2f, 0.2f),
                PromptTheme.Background => (1.0f, 1.0f),
                PromptTheme.Enemy => (0.5f, 0.5f),
                PromptTheme.BossEnemy => (1.1f, 1.1f),
                PromptTheme.Player => (0.6f, 0.6f),
                PromptTheme.UIBackground or PromptTheme.UIButton => (1.0f, 1.0f),
                _ => (1.0f, 1.0f)
            };
    }
}