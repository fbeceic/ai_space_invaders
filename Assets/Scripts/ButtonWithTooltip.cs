using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonWithTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TooltipManager tooltipManager;
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipManager.ShowTooltip(tooltipMessage, Input.mousePosition);
    }

    
    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipManager.HideTooltip();
    }
}