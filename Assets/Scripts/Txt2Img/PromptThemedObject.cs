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

    }
}