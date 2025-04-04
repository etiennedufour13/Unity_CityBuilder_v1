using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class MyTabButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    public TabGroup tabGroup;

    public Image background;

    public bool isOpen = false;

    private Vector2 initialPos;
    public float vertOffset, horOffset;
    public Vector2 normalScale, selectedScale;
    

    //----------------------------------------- Setup ---
        void Start()
    {
        background = GetComponent<Image>();
        tabGroup.Subscribe(this);
        initialPos = transform.localPosition;
    }

    //----------------------------------------- Souris events ---
    public void OnPointerClick(PointerEventData eventData)
    {
        isOpen = !isOpen;
        tabGroup.OnTabSelected(this, isOpen);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }

    //----------------------------------------- Animations ---
    public void ChangePos(bool isUp, int horizontalOffset){
        if (isUp){
            transform.LeanMoveLocal(new Vector2(initialPos.x + horizontalOffset*horOffset, initialPos.y + vertOffset), 0.5f).setEaseOutQuad();
            transform.LeanScale(selectedScale, 0.5f).setEaseOutQuad();

        }
        else{
            transform.LeanMoveLocal(new Vector2(initialPos.x + horizontalOffset*horOffset, initialPos.y), 0.5f).setEaseOutQuad();
            transform.LeanScale(normalScale, 0.5f).setEaseOutQuad();
        }
    }

}
