using UnityEngine;

public class Building : MonoBehaviour
{
    //--------------------------------------- effet imm�diat de placement ---
    public void ApplyEffect(int[] facteurNumber, float[] facteurEffect)
    {
        for (int i = 0; i < facteurNumber.Length; i++)
        {
            CityFactors.Instance.ModifyFactor(facteurNumber[i], facteurEffect[i]);
        }
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
