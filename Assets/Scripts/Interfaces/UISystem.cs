using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance;
    public GameObject panelGauche, panelDroit;
    private BuildingPlacer buildingPlacer;

    //gestion du temps
    public TMP_Text dayText;
    public TMP_Text weekText;
    public Button[] speedButtons;
    public Slider dayProgressSlider;
    private float selectedAlpha = 1f;
    private float unselectedAlpha = 0.2f;




    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        buildingPlacer = FindAnyObjectByType<BuildingPlacer>();
        SetupTime();
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


    //--------------------------------------------------- Gestion du temps ---
    public void SetupTime()
    {
        UpdateDate(TimeManager.Instance.GetDay(), TimeManager.Instance.GetWeek());
        UpdateSpeedButtonVisuals(1); // Play sélectionné par défaut

        for (int i = 0; i < speedButtons.Length; i++)
        {
            int index = i;
            speedButtons[i].onClick.AddListener(() =>
            {
                TimeManager.Instance.SetSpeed(index);
                UpdateSpeedButtonVisuals(index);
            });
        }
    }
    
    public void UpdateDate(int day, int week)
    {
        dayText.text = "Jour " + day;
        weekText.text = "Semaine " + week;
    }

    public void UpdateDayProgress(float progress)
    {
        dayProgressSlider.value = progress;
    }

    private void UpdateSpeedButtonVisuals(int selectedIndex)
    {
        for (int i = 0; i < speedButtons.Length; i++)
        {
            CanvasGroup cg = speedButtons[i].GetComponent<CanvasGroup>();
            if (cg == null) cg = speedButtons[i].gameObject.AddComponent<CanvasGroup>();
            cg.alpha = (i == selectedIndex) ? selectedAlpha : unselectedAlpha;
        }
    }


}
