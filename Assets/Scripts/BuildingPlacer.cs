using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundLayer;

    private GameObject previewInstance;
    private BuildingData selectedBuilding;
    private float rotationY = 0f;

    void Update()
    {
        if (previewInstance != null)
        {
            UpdatePreviewPosition();
            HandleRotation();
            HandlePlacement();
        }
    }

    public void SelectBuilding(BuildingData building)
    {
        if (previewInstance != null) Destroy(previewInstance);
        selectedBuilding = building;
        previewInstance = Instantiate(building.prefab);
        previewInstance.layer = LayerMask.NameToLayer("Ignore Raycast"); // Évite les collisions parasites
    }

    void UpdatePreviewPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            previewInstance.transform.position = hit.point;
            previewInstance.transform.rotation = Quaternion.Euler(0, rotationY, 0);
        }
    }

    void HandleRotation()
    {
        if (Input.GetKey(KeyCode.R))
        {
            rotationY += 100f * Time.deltaTime; // Rotation douce
        }
    }

    void HandlePlacement()
    {
        if (Input.GetMouseButtonDown(0)) // Clic gauche pour placer
        {
            Instantiate(selectedBuilding.prefab, previewInstance.transform.position, previewInstance.transform.rotation);
        }
        else if (Input.GetMouseButtonDown(1)) // Clic droit pour annuler
        {
            Destroy(previewInstance);
            previewInstance = null;
        }
    }
}

