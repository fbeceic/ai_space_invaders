using Txt2Img;
using Txt2Img.Util;
using UnityEngine;

public class OffsetObject : MonoBehaviour
{
    private PromptTheme promptTheme;

    private RectTransform rectTransform;
    void Start()
    {
        promptTheme = GetComponent<PromptThemedObject>().promptTheme;
        rectTransform = GetComponent<RectTransform>();
        
        switch (promptTheme) 
        {
            case PromptTheme.Background:
                ApplyBackgroundOffset();
                break;
            case PromptTheme.Player:
            case PromptTheme.Enemy:
            case PromptTheme.Projectile:
            default:
                ApplyBackgroundOffset();
                break;
        } 
    }

    void ApplyBackgroundOffset()
    {
        Vector3 offsetPosition = new Vector3(500, 345, 0);
        rectTransform.localPosition += offsetPosition;
    }
}