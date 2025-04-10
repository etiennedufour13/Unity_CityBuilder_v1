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
        public float dynamique;
        public Slider slider;
        public TMPro.TextMeshProUGUI text;
    }

    public static CityFactors Instance;

    public CityFactor[] cityFactors;


    // ------------------------------------------- Gestion ---
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

    // ------------------------------------------- Appels ---
    public void ModifyFactor(int factor, float valeur)
    {
        cityFactors[factor].currentValeur += valeur;
        VisualUpdateFactors(factor);
    }

    public void ModifyDynamic(int factor, float valeur){
        cityFactors[factor].dynamique += valeur;
    }

    // ------------------------------------------- Actions ---
    public void ApplyDynamicsToFactors(){
        for (int i = 0; i < cityFactors.Length; i++)
        {
            float currentDynamic = cityFactors[i].dynamique;
            float currentValeurFactor = cityFactors[i].currentValeur;

            int niveau = (int)(Mathf.Abs(currentValeurFactor) / 10);
            if (Mathf.Abs(currentValeurFactor) <= 9) niveau = 0;

            bool versExtremes = (currentValeurFactor > 9 && currentDynamic > 0) || (currentValeurFactor < -9 && currentDynamic < 0);

            if (versExtremes)
            {
                float seuil = Mathf.Pow(2, niveau);
                if (Mathf.Abs(currentDynamic) < seuil) return;

                float modulation = (currentDynamic - seuil) / niveau;
                ModifyFactor(i, modulation);
            }
            else
            {
                ModifyFactor(i, currentDynamic);
            }
        }
    }

    // ------------------------------------------- Visual ---
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
