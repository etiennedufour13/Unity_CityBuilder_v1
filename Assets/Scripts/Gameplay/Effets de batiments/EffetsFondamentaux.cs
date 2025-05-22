using TMPro;
using UnityEngine;

public class EffetsFondamentaux : MonoBehaviour, IBuildingEffect
{
    public float[] fondamentalFactorsDirectImpact;

    public void ApplyEffect()
    {
        for (int i = 0; i < fondamentalFactorsDirectImpact.Length; i++) //parcours les 3 facteurs
        {
            //s'arrète si le facteur est zéro
            if (fondamentalFactorsDirectImpact[i] == 0) break;

            //modification des facteurs de la ville
            CityFactors.Instance.ModifyFactor(i, fondamentalFactorsDirectImpact[i]);

            //initialisation pour l'icone visuelle de factor
            Collider col = GetComponent<Collider>();
            Vector3 basePosition = col.bounds.center + new Vector3(0, col.bounds.extents.y, 0);

            // Calcul direction perpendiculaire droite par rapport à la caméra
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = new Vector3(camForward.z, 0, -camForward.x).normalized;

            // Application de l’offset selon facteurNumber[i]
            float offsetDistance = 1.5f;
            int offsetDir = i - 1; // -1 = gauche, 0 = neutre, 1 = droite
            Vector3 offset = camRight * offsetDistance * offsetDir;
            Vector3 spawnPosition = basePosition + offset;

            //instantiation de l'icone
            GameObject instance = Instantiate(PrefabManager.Instance.mainFactorIcon[i], spawnPosition, Quaternion.identity);
            Destroy(instance, 1f);

            // Scale proportionnelle à la valeur absolue
            float minScale = 0.85f;
            float maxScale = 1.15f;
            float valueAbs = Mathf.Abs(fondamentalFactorsDirectImpact[i]);
            float scale = Mathf.Clamp(minScale + (valueAbs - 1f) * ((maxScale - minScale) / 9f), minScale, maxScale);
            instance.transform.GetChild(0).localScale *= scale; //enfant1
            instance.transform.GetChild(1).localScale *= scale; //enfant2

            // Récupération du Text enfant
            Transform textChild = instance.transform.Find("Text");
            if (textChild != null)
            {
                // Désactivation si valeur absolue == 1
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

        }

    }
}
