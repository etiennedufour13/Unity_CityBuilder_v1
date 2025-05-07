using UnityEngine;

public class BatimentPolluant : MonoBehaviour, IBuildingEffect
{
    //floats à définir
    public float directDySanteImpact;
    public float indirectDySanteImpact;
    public float indirectDySanteRadius;

    //floats de stats actuelle
    [SerializeField]
    public float dySanteImpact;

    //gameObjects
    public GameObject indirectSanteDynamiqueVisualZone;


    //----------------------------------------------------------------------- Direct ---
    void Start()
    {
        //définit la taille de la zone visuelle en fonction du radius
        if (indirectSanteDynamiqueVisualZone != null) indirectSanteDynamiqueVisualZone.transform.localScale *= indirectDySanteRadius;
    }

    public void ApplyEffect()
    {
        //désactive le visuel du radius d'impact
        if (indirectSanteDynamiqueVisualZone != null) indirectSanteDynamiqueVisualZone.SetActive(false);

        //les impacts directs
        // -3 DySanté
        // -3 Santé
    }

    //----------------------------------------------------------------------- Indirect ---
    public float GetDySanteFactor()
    {
        //calcul de l'impact santé
        Collider[] hits = Physics.OverlapSphere(transform.position, indirectDySanteRadius / 2);
        int habitationCount = 0;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Building") && hit.gameObject != this.gameObject && !hit.transform.IsChildOf(this.transform))
            {
                habitationCount++;
            }
        }

        //stat finale remise à jour de l'impact sur la dynamique de santé
        dySanteImpact = directDySanteImpact + (habitationCount * indirectDySanteImpact);
        return dySanteImpact;
    }

    //----------------------------------------------------------------------- Autre ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, indirectDySanteRadius);
    }
}
