using UnityEngine;


public class Building : MonoBehaviour
{

    //--------------------------------------- effet imm�diat de placement ---
    public void PlacementEffects()
    {
        IBuildingEffect[] effects = GetComponents<IBuildingEffect>();
        foreach (IBuildingEffect effect in effects)
        {
            effect.ApplyEffect();
        }

        //effets visuels
        PlacementVisualMovement();
    }

    public void PlacementVisualMovement()
    {
        //effet visuel d'étirement et de tombage pour le placement
        Vector3 normalScale = transform.localScale;
        Vector3 normalPosition = transform.localPosition;

        transform.localScale = new Vector3(normalScale.x, normalScale.y * 1.2f, normalScale.z);
        transform.localPosition = new Vector3(normalPosition.x, normalPosition.y + 2f, normalPosition.z);

        transform.LeanMoveLocal(normalPosition, 0.2f).setEaseOutQuad();
        transform.LeanScale(normalScale, 0.2f).setEaseOutQuad();
    }


    //--------------------------------------- conditions d'overlap (appelées par BuildingPlacer) ---
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
