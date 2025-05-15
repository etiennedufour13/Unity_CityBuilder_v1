using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UISystem : MonoBehaviour
{
    public static UISystem Instance;
    private BuildingPlacer buildingPlacer;

    //gestion du temps
    private int currentSpeedIndex = 1; // 0 = Pause, 1 = x1, 2 = x2, 3 = x4
    public Button pauseButton;
    public Button playButton;
    public Sprite playSprite, x2Sprite, x4Sprite;
    public Image playButtonImage;

    public TMP_Text dayText;
    public TMP_Text weekText;
    public Image radialFillImage;

    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color unselectedColor = new Color(1f, 1f, 1f, 0.5f);


    //barre de nav
    private bool[] tabIsOpen = new bool[4];
    public GameObject[] tabs;
    public GameObject[] openByTabs;
    private Color tabActiveColor = new Color(1f, 1f, 1f, .5f), tabInactiveColor = new Color(0f, 0f, 0f, .5f);



    //--------------------------------------------------- Setup ---
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

    //--------------------------------------------------- Barre de navigation ---
    public void OpenAndClosePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void navButton(int tabIndex)
    {
        //active les visuels et les fenètres (et desactives ceux des autres)
        if (tabIsOpen[tabIndex])
        {
            for (int i = 0; i < tabIsOpen.Length; i++)
            {
                tabIsOpen[i] = false;
                tabs[i].GetComponent<Image>().color = tabInactiveColor;
                if (openByTabs[i] != null) openByTabs[i].SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < tabIsOpen.Length; i++)
            {
                bool isActive = i == tabIndex;
                tabIsOpen[i] = isActive;
                tabs[i].GetComponent<Image>().color = isActive ? tabActiveColor : tabInactiveColor;
                if (openByTabs[i] != null) openByTabs[i].SetActive(isActive);
            }

            if (tabIndex == 3)
            {
                //LancerFonctionDestruction();
            }
        }

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
        SetSpeed(1);

        pauseButton.onClick.AddListener(() =>
        {
            SetSpeed(0);
        });

        playButton.onClick.AddListener(() =>
        {
            if (currentSpeedIndex == 0) currentSpeedIndex = 1;
            else currentSpeedIndex = (currentSpeedIndex % 3) + 1;
            SetSpeed(currentSpeedIndex);
        });
    }

    private void SetSpeed(int index)
    {
        currentSpeedIndex = index;
        TimeManager.Instance.SetSpeed(index);
        UpdatePlayButtonVisual(index);

        Image pauseImg = pauseButton.GetComponent<Image>();
        Image playImg = playButton.GetComponent<Image>();

        if (index == 0)
        {
            pauseImg.color = selectedColor;
            playImg.color = unselectedColor;
        }
        else
        {
            pauseImg.color = unselectedColor;
            playImg.color = selectedColor;
        }
    }

    private void UpdatePlayButtonVisual(int index)
    {
        switch (index)
        {
            case 1:
                playButtonImage.sprite = playSprite;
                break;
            case 2:
                playButtonImage.sprite = x2Sprite;
                break;
            case 3:
                playButtonImage.sprite = x4Sprite;
                break;
            default:
                playButtonImage.sprite = playSprite;
                break;
        }
    }

    public void UpdateDate(int day, int week)
    {
        dayText.text = day.ToString();
        weekText.text = "Semaine " + week;
    }

    public void UpdateDayProgress(float progress)
    {
        radialFillImage.fillAmount = Mathf.Clamp01(progress);
    }
}
