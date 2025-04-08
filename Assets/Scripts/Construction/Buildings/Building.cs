using UnityEngine;
using TMPro;

public class Building : MonoBehaviour
{

    //--------------------------------------- effet imm�diat de placement sur les facteurs---
    public void ApplyEffect(int[] facteurNumber, float[] facteurEffect)
    {
        for (int i = 0; i < facteurNumber.Length; i++)
        {
            //modification des facteurs de la ville
            CityFactors.Instance.ModifyFactor(facteurNumber[i], facteurEffect[i]);

            //icone visuelle de factor
            if (facteurEffect[i] != 0 && (facteurNumber[i] <= 2)) {
                Collider col = GetComponent<Collider>();

                Vector3 basePosition = col.bounds.center + new Vector3(0, col.bounds.extents.y, 0);

                // Calcul direction perpendiculaire droite par rapport à la caméra
                Vector3 camForward = Camera.main.transform.forward;
                Vector3 camRight = new Vector3(camForward.z, 0, -camForward.x).normalized;

                // Application de l’offset selon facteurNumber[i]
                float offsetDistance = 1.5f;
                int offsetDir = facteurNumber[i] - 1; // -1 = gauche, 0 = neutre, 1 = droite
                Vector3 offset = camRight * offsetDistance * offsetDir;

                Vector3 spawnPosition = basePosition + offset;


                GameObject instance = Instantiate(PrefabManager.Instance.mainFactorIcon[i], spawnPosition, Quaternion.identity);
                Destroy(instance, 1f);

                // Scale proportionnelle à la valeur absolue
                float minScale = 0.85f;
                float maxScale = 1.15f;
                float valueAbs = Mathf.Abs(facteurEffect[i]);
                float scale = Mathf.Clamp(minScale + (valueAbs - 1f) * ((maxScale - minScale) / 9f), minScale, maxScale);
                instance.transform.GetChild(0).localScale *= scale; //enfant1
                instance.transform.GetChild(1).localScale *= scale; //enfant2

                // Récupération du Text enfant
                Transform textChild = instance.transform.Find("Text");
                if (textChild != null) {
                    // Désactivation si valeur absolue == 1
                    textChild.gameObject.SetActive(valueAbs != 1);

                    // Modification du texte
                    if (valueAbs != 1) {
                        TextMeshPro tmp = textChild.GetComponent<TextMeshPro>();
                        if (tmp != null) {
                            tmp.text = (facteurEffect[i] > 0 ? "+" : "-") + valueAbs.ToString();
                        }
                    }
                }
            }

        }
    }

    //--------------------------------------- effet visuel de placement ---
    public void PlacementEffects()
    {
        Vector3 normalScale = transform.localScale;
        Vector3 normalPosition = transform.localPosition;

        transform.localScale = new Vector3(normalScale.x, normalScale.y * 1.2f, normalScale.z);
        transform.localPosition = new Vector3(normalPosition.x, normalPosition.y + 2f, normalPosition.z);

        transform.LeanMoveLocal(normalPosition, 0.2f).setEaseOutQuad();
        transform.LeanScale(normalScale, 0.2f).setEaseOutQuad();
    }


    //--------------------------------------- syst�me de placement ---
    public bool IsValidPlacement()
    {
        return !CheckOverlap();
    }

    private bool CheckOverlap()
    {
        Collider collider = GetComponent<Collider>();
        if (collider == null) return true;

        Collider[] colliders = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, transform.rotation);
        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject) // Ignore soi-m�me
            {
                return true;
            }
        }
        return false;
    }
}
