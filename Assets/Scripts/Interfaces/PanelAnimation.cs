using System.Collections;
using UnityEngine;

public class PanelAnimation : MonoBehaviour
{
    private Vector2 initialPos;
    public Vector2 posOffset;

    void Start(){
        transform.localScale = Vector2.zero;
        initialPos = transform.localPosition;
        transform.localPosition = initialPos -posOffset;
    }

    public void Open(){
        transform.LeanScale(Vector2.one, 0.1f);
        transform.LeanMoveLocal(initialPos, 0.1f);
    }

    public void Close(){
        transform.LeanScale(Vector2.zero, 0.1f);
        transform.LeanMoveLocal(initialPos -posOffset, 0.1f);
    }
}
