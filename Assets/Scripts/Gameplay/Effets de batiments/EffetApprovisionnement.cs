using UnityEngine;
using System.Collections.Generic;

public class EffetApprovisionnement : MonoBehaviour, IBuildingEffect
{
    //floats � d�finir
    public float directDyApprovisionnementImpact;
    public float indirectDyApprovisionnementImpact;
    public float indirectDyApprovisionnementRadius;

    //floats de stats actuelle
    [SerializeField]
    public float dyApprovisionnementImpact;

    //gameObjects
    public GameObject indirectDyApprovisionnementZone;

    //gestion visuelle de la zone
    private bool zoneActive = true;
    public Color zoneColor;
    private HashSet<GameObject> buildingsInZone = new HashSet<GameObject>();


    //----------------------------------------------------------------------- Direct ---
    void Start()
    {
        //zone visuelle d'impact
        if (indirectDyApprovisionnementZone != null){
            indirectDyApprovisionnementZone.transform.localScale *= indirectDyApprovisionnementRadius;
        }
    }

    public void ApplyEffect()
    {
        //d�sactive le visuel du radius d'impact
        if (indirectDyApprovisionnementZone != null) indirectDyApprovisionnementZone.SetActive(false);
        ClearAllOutlines();

        //les impacts directs
    }

    //----------------------------------------------------------------------- Indirect ---
    public float GetDySanteFactor()
    {
        //calcul de l'impact sant�
        Collider[] hits = Physics.OverlapSphere(transform.position, indirectDyApprovisionnementRadius / 2);
        int habitationCount = 0;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Building") && hit.gameObject != this.gameObject && !hit.transform.IsChildOf(this.transform))
            {
                habitationCount++;
            }
        }

        //stat finale remise � jour de l'impact sur la dynamique de sant�
        dyApprovisionnementImpact = directDyApprovisionnementImpact + (habitationCount * indirectDyApprovisionnementImpact);
        return dyApprovisionnementImpact;
    }

    //----------------------------------------------------------------------- Gestion des outlines d'impact ---
    void Update()
    {
        if (indirectDyApprovisionnementZone == null){
            return;
        }
        else if (!indirectDyApprovisionnementZone.activeSelf && zoneActive)
        {
            ClearAllOutlines();
            return;
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, indirectDyApprovisionnementRadius);
        HashSet<GameObject> currentHits = new HashSet<GameObject>();

        foreach (var hit in hits)
        {
            GameObject obj = hit.gameObject;

            if (obj.CompareTag("Building") && obj != this.gameObject && !obj.transform.IsChildOf(this.transform))
            {
                currentHits.Add(obj);

                if (!buildingsInZone.Contains(obj))
                {
                    MaterialController mc = obj.GetComponent<MaterialController>();
                    if (mc != null)
                        mc.SetOutline(true, zoneColor);
                }
            }
        }

        // Remove outline from objects that exited the zone
        foreach (GameObject obj in buildingsInZone)
        {
            if (!currentHits.Contains(obj))
            {
                MaterialController mc = obj.GetComponent<MaterialController>();
                if (mc != null)
                    mc.SetOutline(false, zoneColor);
            }
        }

        buildingsInZone = currentHits;
    }

    private void ClearAllOutlines()
    {
        zoneActive = false;

        foreach (GameObject obj in buildingsInZone)
        {
            MaterialController mc = obj.GetComponent<MaterialController>();
            if (mc != null)
                mc.SetOutline(false, zoneColor);
        }

        buildingsInZone.Clear();
    }

    //----------------------------------------------------------------------- Autre ---
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, indirectDyApprovisionnementRadius);
    }
}