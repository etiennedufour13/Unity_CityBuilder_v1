using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<MyTabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;
    public MyTabButton selectedTab;
    public List<GameObject> objectsToSwap;


    public void Subscribe(MyTabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<MyTabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(MyTabButton button)
    {
        ResetTabs();
        if (selectedTab != null && button != selectedTab) {
            button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(MyTabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(MyTabButton button)
    {
        selectedTab = button;
        ResetTabs();
        button.background.sprite = tabActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++) { 
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
            }
            else
            {
                objectsToSwap[i].SetActive(false );
            }
        }
    }

    public void ResetTabs()
    {
        foreach (MyTabButton button in tabButtons) {
            if (selectedTab != null && button == selectedTab) { continue;  }
            button.background.sprite = tabIdle;
        }
    }
}
