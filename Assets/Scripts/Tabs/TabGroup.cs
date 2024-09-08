using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons = new List<TabButton>();
    public List<GameObject> tabPages = new List<GameObject>();

    public Color tabIdleColor;
    public Color tabHoverColor;
    public Color tabSelectedColor;
    
    public TabButton selectedTab;
    private TabButton lastSelectedTab;

    public void Start()
    {
        tabButtons.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
        foreach (TabButton tabButton in tabButtons)
        {
            if (tabButton.transform.GetSiblingIndex() == 0)
                OnTabSelected(tabButton);
        }            
    }

    public void OnTabEnter(TabButton tabButton)
    {
        ResetTabs();
        if ((selectedTab == null) || (tabButton != selectedTab))
            tabButton.background.color = tabHoverColor;
    }

    public void OnTabExit(TabButton tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton tabButton)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = tabButton;

        selectedTab.Select();

        ResetTabs();
        tabButton.background.color = tabSelectedColor;
        int index = tabButton.transform.GetSiblingIndex();
        for (int i = 0; i < tabPages.Count; i++)
        {
            if (i == index)
            {
                tabPages[i].SetActive(true);
            }
            else
            {
                tabPages[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach(TabButton tabButton in tabButtons)
        {
            if ((selectedTab != null) && (tabButton == selectedTab))
                continue;
            tabButton.background.color = tabIdleColor;
        }
    }

    public void NextTab()
    {
        int currentIndex = selectedTab.transform.GetSiblingIndex();
        int nextIndex = currentIndex < tabButtons.Count - 1 ? currentIndex + 1 : tabButtons.Count - 1;
        OnTabSelected(tabButtons[nextIndex]);
    }

    public void PreviousTab()
    {
        int currentIndex = selectedTab.transform.GetSiblingIndex();
        int previousIndex = currentIndex > 0 ? currentIndex - 1 : 0;
        OnTabSelected(tabButtons[previousIndex]);
    }

    public void ToElementsTab()
    {
        if (tabButtons.Count >= 0)
        {
            OnTabSelected(lastSelectedTab); 
        }
        else
        {
            Debug.LogWarning("There are not enough tabs to switch to the third one.");
        }
    }

    public void ToGalleryTab(TabButton fromTab)
    {
        lastSelectedTab = fromTab;
        
        if (tabButtons.Count >= 3)
        {
            OnTabSelected(tabButtons[2]); 
        }
        else
        {
            Debug.LogWarning("There are not enough tabs to switch to the third one.");
        }
    }
}
