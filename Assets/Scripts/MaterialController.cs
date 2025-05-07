using UnityEngine;

public class MaterialController : MonoBehaviour
{
    private GameObject outlineObject, filterObject;
    public Color hoverColor;


    void Start()
    {
        outlineObject = transform.Find("Outline")?.gameObject;
        filterObject = transform.Find("Filtre")?.gameObject;
    }

    public void SetOutline(bool OnOff, Color color)
    {
        if (outlineObject != null && filterObject != null){
            // outline
            outlineObject.SetActive(OnOff);
            Renderer outLineRenderer = outlineObject.GetComponent<Renderer>();
            Material[] outlineMaterials = outLineRenderer.materials;

            for (int i = 0; i < outlineMaterials.Length; i++)
            {
                outlineMaterials[i].SetColor("_OutlineColor", color);
            }
            outLineRenderer.materials = outlineMaterials;

            //filtre
            filterObject.SetActive(OnOff);
            Renderer filtreRenderer = filterObject.GetComponent<Renderer>();
            Material[] filtreMaterials = filtreRenderer.materials;

            for (int i = 0; i < filtreMaterials.Length; i++)
            {
                filtreMaterials[i].SetColor("_OutlineColor", color);
            }
            filtreRenderer.materials = filtreMaterials;
        }
    }

    public void SetCollisionState(bool inCollision)
    {
        //a suppr

    }

    public void SetTransparent(bool inCollision)
    {
        //a suppr

    }
}
