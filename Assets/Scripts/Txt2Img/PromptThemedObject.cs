using Txt2Img.Util;
using UnityEngine;
using UnityEngine.UI;

namespace Txt2Img
{
    public class PromptThemedObject : MonoBehaviour
    {
        [SerializeField] public PromptTheme promptTheme;
        
        private void Start()
        {
            ApplyPromptResult();
            if (promptTheme != PromptTheme.Background)
            {
                ReinitializeCollider();
            }
        }

        private void ApplyPromptResult()
        {
            var generatedSprite = PromptHelper.GetPromptResult(promptTheme);

            var spriteRenderer = GetComponent<SpriteRenderer>();
            var image = GetComponent<Image>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = generatedSprite;
            } else if (image != null)
            {
                image.sprite = generatedSprite;
            }
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
    }
}