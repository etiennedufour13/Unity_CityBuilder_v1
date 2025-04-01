using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CityFactors;
using static System.Net.Mime.MediaTypeNames;

public class CityFactors : MonoBehaviour
{
    [System.Serializable]
    public class CityFactor
    {
        public string factorName;
        public float valeurInitiale;
        public float currentValeur;
        public Slider slider;
        public TMPro.TextMeshProUGUI text;
    }

    public static CityFactors Instance;

    public CityFactor[] cityFactors;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        //initialisation
        for (int i = 0; i < cityFactors.Length; i++)
        {
            ModifyFactor(i, cityFactors[i].valeurInitiale);
        }
    }


    public void ModifyFactor(int factor, float valeur)
    {
        cityFactors[factor].currentValeur += valeur;
        VisualUpdateFactors(factor);
    }

    private void VisualUpdateFactors(int factor)
    {
        if (cityFactors[factor].slider != null){
            cityFactors[factor].slider.value = cityFactors[factor].currentValeur;
        }
        if (cityFactors[factor].text != null)
        {
            cityFactors[factor].text.text = cityFactors[factor].currentValeur.ToString();
        }
    }
}
