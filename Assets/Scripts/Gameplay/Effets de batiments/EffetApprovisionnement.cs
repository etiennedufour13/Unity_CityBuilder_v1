using UnityEngine;

public class EffetApprovisionnement : MonoBehaviour, IBuildingEffect
{
    //floats à définir
    public float directDyApprovisionnementImpact;
    public float indirectDyApprovisionnementImpact;
    public float indirectDyApprovisionnementRadius;

    //floats de stats actuelle
    [SerializeField]
    public float dyApprovisionnementImpact;

    //gameObjects
    public GameObject indirectDyApprovisionnementZone;


    //----------------------------------------------------------------------- Direct ---
    void Start()
    {
        //définit la taille de la zone visuelle en fonction du radius
        if (indirectDyApprovisionnementZone != null) indirectDyApprovisionnementZone.transform.localScale *= indirectDyApprovisionnementRadius;
    }

    public void ApplyEffect()
    {
        //désactive le visuel du radius d'impact
        if (indirectDyApprovisionnementZone != null) indirectDyApprovisionnementZone.SetActive(false);

        //les impacts directs

    }

    //----------------------------------------------------------------------- Indirect ---
    public float GetDySanteFactor()
    {
        //calcul de l'impact santé
        Collider[] hits = Physics.OverlapSphere(transform.position, indirectDyApprovisionnementRadius / 2);
        int habitationCount = 0;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Building") && hit.gameObject != this.gameObject && !hit.transform.IsChildOf(this.transform))
            {
                habitationCount++;
            }
        }

        //stat finale remise à jour de l'impact sur la dynamique de santé
        dyApprovisionnementImpact = directDyApprovisionnementImpact + (habitationCount * indirectDyApprovisionnementImpact);
        return dyApprovisionnementImpact;
    }

    //----------------------------------------------------------------------- Autre ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, indirectDyApprovisionnementRadius);
    }
}