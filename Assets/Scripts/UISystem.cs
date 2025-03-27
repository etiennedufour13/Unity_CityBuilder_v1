using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public GameObject panelGauche, panelDroit;
    private BuildingPlacer buildingPlacer;

    void Start()
    {
        buildingPlacer = FindAnyObjectByType<BuildingPlacer>();
    }

    public void OpenAndClosePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    //--------------------------------------------------- Paramètres de placement ---
    public void GridButton(){
        buildingPlacer.ToggleGrid();
    }

    public void CranButton(){
        buildingPlacer.ToggleSnapRotation();
    }
}
