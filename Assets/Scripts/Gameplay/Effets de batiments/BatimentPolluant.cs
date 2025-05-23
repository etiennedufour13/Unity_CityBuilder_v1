using UnityEngine;

public class BatimentPolluant : MonoBehaviour, IBuildingEffect
{
    //floats � d�finir
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
        //d�finit la taille de la zone visuelle en fonction du radius
        if (indirectSanteDynamiqueVisualZone != null) indirectSanteDynamiqueVisualZone.transform.localScale *= indirectDySanteRadius;
    }

    public void ApplyEffect()
    {
        //d�sactive le visuel du radius d'impact
        if (indirectSanteDynamiqueVisualZone != null) indirectSanteDynamiqueVisualZone.SetActive(false);

        //les impacts directs
        // -3 DySant�
        // -3 Sant�
    }

    //----------------------------------------------------------------------- Indirect ---
    public float GetDySanteFactor()
    {
        //calcul de l'impact sant�
        Collider[] hits = Physics.OverlapSphere(transform.position, indirectDySanteRadius / 2);
        int habitationCount = 0;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Building") && hit.gameObject != this.gameObject && !hit.transform.IsChildOf(this.transform))
            {
                habitationCount++;
            }
        }

        //stat finale remise � jour de l'impact sur la dynamique de sant�
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
