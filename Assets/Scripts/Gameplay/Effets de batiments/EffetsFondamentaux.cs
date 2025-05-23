using TMPro;
using UnityEngine;
using TMPro;


public class EffetsFondamentaux : MonoBehaviour, IBuildingEffect
{
    public float[] fondamentalFactorsDirectImpact;

    public GameObject[] icons;
    public Color[] iconColors;
    private bool showPlus = false;

    public void ApplyEffect()
    {
        for (int i = 0; i < fondamentalFactorsDirectImpact.Length; i++) //parcours les 3 facteurs
        {
            //s'arr�te si le facteur est z�ro
            if (fondamentalFactorsDirectImpact[i] == 0) break;

            //modification des facteurs de la ville
            CityFactors.Instance.ModifyFactor(i, fondamentalFactorsDirectImpact[i]);

            //initialisation pour l'icone visuelle de factor
            Collider col = GetComponent<Collider>();
            Vector3 basePosition = col.bounds.center + new Vector3(0, col.bounds.extents.y, 0);

            // Calcul direction perpendiculaire droite par rapport � la cam�ra
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = new Vector3(camForward.z, 0, -camForward.x).normalized;

            // Application de l�offset selon facteurNumber[i]
            float offsetDistance = 1.5f;
            int offsetDir = i - 1; // -1 = gauche, 0 = neutre, 1 = droite
            Vector3 offset = camRight * offsetDistance * offsetDir;
            Vector3 spawnPosition = basePosition + offset;

            //instantiation de l'icone
            GameObject instance = Instantiate(PrefabManager.Instance.mainFactorIcon[i], spawnPosition, Quaternion.identity);
            Destroy(instance, 1f);

            // Scale proportionnelle � la valeur absolue
            float minScale = 0.85f;
            float maxScale = 1.15f;
            float valueAbs = Mathf.Abs(fondamentalFactorsDirectImpact[i]);
            float scale = Mathf.Clamp(minScale + (valueAbs - 1f) * ((maxScale - minScale) / 9f), minScale, maxScale);
            instance.transform.GetChild(0).localScale *= scale; //enfant1
            instance.transform.GetChild(1).localScale *= scale; //enfant2

            // R�cup�ration du Text enfant
            Transform textChild = instance.transform.Find("Text");
            if (textChild != null)
            {
                // D�sactivation si valeur absolue == 1
                textChild.gameObject.SetActive(valueAbs != 1);

                // Modification du texte
                if (valueAbs != 1)
                {
                    TextMeshPro tmp = textChild.GetComponent<TextMeshPro>();
                    if (tmp != null)
                    {
                        tmp.text = (fondamentalFactorsDirectImpact[i] > 0 ? "+" : "-") + valueAbs.ToString();
                    }
                }
            }

            //lance l'anim des icones
            icons[i].GetComponent<IconAnimation>().StartAnimation();

        }

    }


    void Start()
    {
        for (int i = 0; i < icons.Length; i++)
        {
            if (i >= iconColors.Length || i >= fondamentalFactorsDirectImpact.Length) break;

            SpriteRenderer sr = icons[i].GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = iconColors[i];

            Transform child = icons[i].transform.childCount > 0 ? icons[i].transform.GetChild(0) : null;
            if (child != null)
            {
                TextMeshPro tmp = child.GetComponent<TextMeshPro>();
                if (tmp != null)
                {
                    if (fondamentalFactorsDirectImpact[i] > 0 && showPlus)
                    {
                        tmp.text = "+" + fondamentalFactorsDirectImpact[i].ToString();
                    }
                    else
                    {
                        tmp.text = fondamentalFactorsDirectImpact[i].ToString();
                    }
                }

            }
        }
    }
}
