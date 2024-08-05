using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipManager : MonoBehaviour
{
    public GameObject tooltip;
    public TextMeshProUGUI tooltipText;

    void Start()
    {
        HideTooltip();
    }

    public void ShowTooltip(string message, Vector3 position)
    {
        tooltip.SetActive(true);
        tooltipText.text = message;
        tooltip.transform.position = position;
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}