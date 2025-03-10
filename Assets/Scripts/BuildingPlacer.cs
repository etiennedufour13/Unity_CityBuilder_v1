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

    // script lancé par le bouton dans l'interface : création de la preview
    public void SelectBuilding(BuildingData building) //building = préfab du building choisit
    {
        //création de la nouvelle instance
        if (previewInstance != null) Destroy(previewInstance);
        selectedBuilding = building;
        previewInstance = Instantiate(building.prefab);
        //previewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");

        //reliage avec le système visuel de la preview
        //délais pour éviter un bug (de chargement trop tôt)
        StartCoroutine(InitializeMaterialController());
    }
        private IEnumerator InitializeMaterialController()
        {
            yield return null; // Attend un frame pour que Start() dans MaterialController s'exécute

            materialController = previewInstance.GetComponentInChildren<MaterialController>();

            if (materialController != null)
            {
                materialController.SetTransparent(true);
            }
            else
            {
                Debug.LogError("MaterialController introuvable sur le prefab sélectionné !");
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

    //vérification des collisions de placement
    bool CheckPlacementValidity()
    {
        Collider previewCollider = previewInstance.GetComponent<Collider>();
        if (previewCollider == null) return false;

        Collider[] colliders = Physics.OverlapBox(previewCollider.bounds.center, previewCollider.bounds.extents, previewInstance.transform.rotation);

        foreach (Collider col in colliders)
        {
            if (col.gameObject != previewInstance && col.CompareTag("Building")) // Vérifie uniquement les bâtiments
            {
                return false; // Placement invalide
            }
        }
        return true; // Placement valide
    }

}
