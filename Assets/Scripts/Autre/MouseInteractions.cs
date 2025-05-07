using UnityEngine;

public class MouseInteractions : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BuildingPlacer buildingPlacer;
    [SerializeField] private Color hoverColor;

    private GameObject lastBuildingHit;


    // script qui fait un effet de hover en activant Ouline et Filtre des batiments qu'il croise
    // s'assure qu'un batiment n'est pas en cours de construction (pour ne pas lui faire un hover lui même)
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag("Building") && buildingPlacer.previewInstance == null)
            {
                if (hitObject != lastBuildingHit)
                {
                    ResetLastHit();

                    MaterialController matController = hitObject.GetComponent<MaterialController>();
                    if (matController != null)
                    {
                        matController.SetOutline(true, hoverColor);
                        lastBuildingHit = hitObject;
                    }
                }
            }
            else
            {
                ResetLastHit();
            }
        }
        else
        {
            ResetLastHit();
        }
    }

    private void ResetLastHit()
    {
        if (lastBuildingHit != null)
        {
            MaterialController matController = lastBuildingHit.GetComponent<MaterialController>();
            if (matController != null)
            {
                matController.SetOutline(false, hoverColor);
            }

            lastBuildingHit = null;
        }
    }
}
