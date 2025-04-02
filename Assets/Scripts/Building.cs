using UnityEngine;

public class Building : MonoBehaviour
{


    //--------------------------------------- effet immédiat de placement ---
    public void ApplyEffect(int[] facteurNumber, float[] facteurEffect)
    {
        for (int i = 0; i < facteurNumber.Length; i++)
        {
            //modification des facteurs de la ville
            CityFactors.Instance.ModifyFactor(facteurNumber[i], facteurEffect[i]);

            //icone visuelle de factor
            Collider col = GetComponent<Collider>();
            Vector3 spawnPosition = col.bounds.center + new Vector3(0, col.bounds.extents.y, 0);
            Instantiate(PrefabManager.Instance.ecoIcon, spawnPosition, Quaternion.identity);
        }
    }

    //--------------------------------------- système de placement ---
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
            if (col.gameObject != gameObject) // Ignore soi-même
            {
                return true;
            }
        }
        return false;
    }
}
