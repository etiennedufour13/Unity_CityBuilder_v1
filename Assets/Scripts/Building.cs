using UnityEngine;

public class Building : MonoBehaviour
{
    //--------------------------------------- effet immédiat de placement ---
    public void ApplyEffect(int[] facteurNumber, float[] facteurEffect)
    {
        for (int i = 0; i < facteurNumber.Length; i++)
        {
            CityFactors.Instance.ModifyFactor(facteurNumber[i], facteurEffect[i]);
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
