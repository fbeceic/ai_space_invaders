using TMPro;
using UnityEngine;  
using UnityEngine.EventSystems;

public class DynamicButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public TextMeshProUGUI theText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        theText.color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        theText.color = Color.black;
    }
}