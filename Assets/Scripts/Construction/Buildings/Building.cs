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
            if (facteurEffect[i] != 0) {
                Collider col = GetComponent<Collider>();
                Vector3 spawnPosition = col.bounds.center + new Vector3(0, col.bounds.extents.y, 0) + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

                GameObject instance = Instantiate(PrefabManager.Instance.mainFactorIcon[i], spawnPosition, Quaternion.identity);
                Destroy(instance, 1f);

                // Scale proportionnelle à la valeur absolue
                float valueAbs = Mathf.Abs(facteurEffect[i]);
                float scale = Mathf.Clamp(0.5f + (valueAbs - 1f) * (1.5f / 9f), 0.5f, 1f);
                instance.transform.localScale = Vector3.one * scale;

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
