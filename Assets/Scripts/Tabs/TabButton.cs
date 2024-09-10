using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    public UnityEvent onTabSelected;
    public UnityEvent onTabDeselected;

    private AudioSource audioSource;

    [HideInInspector] public Image background;

    void Awake()
    {
        background = GetComponent<Image>();
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (AIManager.Instance.editingPromptResult == null)
        {
            tabGroup.OnTabSelected(this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (AIManager.Instance.editingPromptResult == null)
        {
            tabGroup.OnTabEnter(this);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (AIManager.Instance.editingPromptResult == null)
        {
            tabGroup.OnTabExit(this);
        }
    }

    public void Select()
    {
        if (onTabSelected != null)
        {
            audioSource.Play();
            onTabSelected.Invoke();
        }
    }

    public void Deselect()
    {
        if (onTabDeselected != null)
        {
            onTabSelected.Invoke();
        }
    }
}