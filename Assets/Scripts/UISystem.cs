using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public GameObject panelGauche, panelDroit;

    public void OpenAndClosePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }
}
