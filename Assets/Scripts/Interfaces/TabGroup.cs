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


    //ajout des tabs enfants
    public void Subscribe(MyTabButton button)
    {
        //ajout manuel au final
    }

    //la souris passe au dessus
    public void OnTabEnter(MyTabButton button)
    {
        ResetTabs();
        if (selectedTab != null && button != selectedTab) {
            button.background.sprite = tabHover;
        }
    }

    //la souris n'est plus au dessus
    public void OnTabExit(MyTabButton button)
    {
        ResetTabs();
    }

    //clic du bouton
    public void OnTabSelected(MyTabButton button, bool isActive)
    {
        selectedTab = button;
        ResetTabs(); // passe les visuels à neutre
        int index = button.transform.GetSiblingIndex(); //trouve l'index du bouton

        if (isActive){
            button.background.sprite = tabActive;
            for (int i = 0; i < objectsToSwap.Count; i++) { 
                if (i == index)
                {
                    objectsToSwap[i].GetComponent<PanelAnimation>().Open();
                }
                else
                {
                    objectsToSwap[i].GetComponent<PanelAnimation>().Close();                   
                }
            }

        }
        else {
            objectsToSwap[index].GetComponent<PanelAnimation>().Close();   
        }


        //lance les animations
        int selectedIndex = tabButtons.IndexOf(button);

        foreach (MyTabButton tabButton in tabButtons) //desactive les autres
        {
            //actif
            if (tabButton == button)
                tabButton.ChangePos(isActive, 0);
            else
            {
                tabButton.isOpen = false;

                int direction = 0;
                if (isActive)
                    direction = (tabButtons.IndexOf(tabButton) < selectedIndex) ? -1 : 1;

                tabButton.ChangePos(false, direction);
            }
        }
    }

    //passe les visuels à neutres
    public void ResetTabs()
    {
        foreach (MyTabButton button in tabButtons) {
            if (selectedTab != null && button == selectedTab) { continue;  }
            button.background.sprite = tabIdle;
        }
    }
}
