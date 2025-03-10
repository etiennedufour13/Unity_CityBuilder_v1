using UnityEngine;
using System.Collections;

public class BuildingPlacer : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundLayer;

    private GameObject previewInstance;
    private BuildingData selectedBuilding;
    private float rotationY = 0f;
    private bool isValidPlacement = false;
    private MaterialController materialController;

    void Update()
    {
        if (previewInstance != null)
        {
            UpdatePreviewPosition();
            HandleRotation();
            HandlePlacement();
        }
    }

    // script lanc� par le bouton dans l'interface : cr�ation de la preview
    public void SelectBuilding(BuildingData building) //building = pr�fab du building choisit
    {
        //cr�ation de la nouvelle instance
        if (previewInstance != null) Destroy(previewInstance);
        selectedBuilding = building;
        previewInstance = Instantiate(building.prefab);
        //previewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");

        //reliage avec le syst�me visuel de la preview
        //d�lais pour �viter un bug (de chargement trop t�t)
        StartCoroutine(InitializeMaterialController());
    }
        private IEnumerator InitializeMaterialController()
        {
            yield return null; // Attend un frame pour que Start() dans MaterialController s'ex�cute

            materialController = previewInstance.GetComponentInChildren<MaterialController>();

            if (materialController != null)
            {
                materialController.SetTransparent(true);
            }
            else
            {
                Debug.LogError("MaterialController introuvable sur le prefab s�lectionn� !");
            }
        }

    // suivit de la preview sur la souris
    void UpdatePreviewPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            previewInstance.transform.position = hit.point;
            previewInstance.transform.rotation = Quaternion.Euler(0, rotationY, 0);

            isValidPlacement = CheckPlacementValidity();
            if (materialController != null) materialController.SetCollisionState(!isValidPlacement);
        }
    }

    //input de rotation
    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.R))
        {
            rotationY += 100f * Time.deltaTime;
        }
    }

    //input de rotation
    void HandlePlacement()
    {
        if (Input.GetMouseButtonDown(0) && isValidPlacement)
        {
            Instantiate(selectedBuilding.prefab, previewInstance.transform.position, previewInstance.transform.rotation);
            Destroy(previewInstance);
            previewInstance = null;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }

    //v�rification des collisions de placement
    bool CheckPlacementValidity()
    {
        Collider previewCollider = previewInstance.GetComponent<Collider>();
        if (previewCollider == null) return false;

        Collider[] colliders = Physics.OverlapBox(previewCollider.bounds.center, previewCollider.bounds.extents, previewInstance.transform.rotation);

        foreach (Collider col in colliders)
        {
            if (col.gameObject != previewInstance && col.CompareTag("Building")) // V�rifie uniquement les b�timents
            {
                return false; // Placement invalide
            }
        }
        return true; // Placement valide
    }

}
