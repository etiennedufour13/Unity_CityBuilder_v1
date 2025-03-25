using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BuildingPlacer : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundLayer;

    private GameObject previewInstance;
    private BuildingData selectedBuilding;
    private float rotationY = 0f;
    private bool isValidPlacement = false;
    private MaterialController materialController;
    public List<string> forbiddenTags = new List<string> { "Building", "Path" };
    private HashSet<GameObject> overlappingObjects = new HashSet<GameObject>();

    private bool snapRotationEnabled = false; //système de crantage
    public float snapAngle = 30f; //degré de crantage
    float rotationSpeed = 500f; // Vitesse en degrés par seconde
    float targetRotationY; // pour stocket la rotation de manière temporaire

    public bool gridPlacementEnabled;
    int buildingWidth, buildingLength; // utilisé pour la grille
    public float gridSize = 1f; //taille de la grille


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
        //changement du layermask pour ne pas capter le raycast
        previewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
        foreach (Transform child in transform) child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        Rigidbody rb = previewInstance.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        //récupération de la taille du bat
        buildingWidth = selectedBuilding.gridWidth;
        buildingLength = selectedBuilding.gridLength;


        Collider previewCollider = previewInstance.GetComponent<Collider>();
        if (previewCollider != null)
        {
            previewCollider.isTrigger = true;
        }

        StartCoroutine(InitializeMaterialController());
    }

    private IEnumerator InitializeMaterialController()
    {
        yield return null;
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

    void UpdatePreviewPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;

            if (gridPlacementEnabled)
            {
                float gridSize = 1f; // Taille de la grille, à ajuster selon ton jeu
                int buildingWidth = selectedBuilding.gridWidth;  // Largeur du bâtiment en unités de grille
                int buildingLength = selectedBuilding.gridLength; // Longueur du bâtiment en unités de grille
                
                // Ajuster la position pour centrer le bâtiment sur la grille
                float snappedX = Mathf.Round(targetPosition.x / gridSize) * gridSize;
                float snappedZ = Mathf.Round(targetPosition.z / gridSize) * gridSize;

                // Décaler la position pour aligner correctement les bâtiments de taille variable
                snappedX += ((buildingWidth % 2) == 0) ? gridSize / 2f : 0;
                snappedZ += ((buildingLength % 2) == 0) ? gridSize / 2f : 0;

                targetPosition = new Vector3(snappedX, targetPosition.y, snappedZ);
            }

            previewInstance.transform.position = targetPosition;
            previewInstance.transform.rotation = Quaternion.Euler(0, rotationY, 0);

            overlappingObjects.Clear();
            BoxCollider boxCol = previewInstance.GetComponent<BoxCollider>();
            if (boxCol != null)
            {
                Vector3 worldCenter = previewInstance.transform.TransformPoint(boxCol.center);
                Vector3 halfExtents = Vector3.Scale(boxCol.size, previewInstance.transform.lossyScale) * 0.5f;
                Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, previewInstance.transform.rotation);

                foreach (var hitCol in hits)
                {
                    if (!hitCol.transform.IsChildOf(previewInstance.transform) && forbiddenTags.Contains(hitCol.tag))
                    {
                        overlappingObjects.Add(hitCol.gameObject);
                    }
                }
            }

            isValidPlacement = overlappingObjects.Count == 0;
            if (materialController != null) materialController.SetCollisionState(!isValidPlacement);
        }
    }


    void HandleRotation()
    {
        if (snapRotationEnabled){
            if (Input.GetKeyDown(KeyCode.R) && !Input.GetKey(KeyCode.LeftShift))
            {
                targetRotationY = Mathf.Round((targetRotationY + snapAngle) / snapAngle) * snapAngle;
            }
            else if (Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
            {
                targetRotationY = Mathf.Round((targetRotationY - snapAngle) / snapAngle) * snapAngle;
            }
        }
        else{
            if (Input.GetKey(KeyCode.R) && !Input.GetKey(KeyCode.LeftShift))
            {
                targetRotationY += 100f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
            {
                targetRotationY += -100f * Time.deltaTime;
            }
        }

        rotationY = Mathf.MoveTowardsAngle(rotationY, targetRotationY, rotationSpeed * Time.deltaTime);

    }

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

    void OnDrawGizmos()
    {
        if (previewInstance != null)
        {
            BoxCollider boxCol = previewInstance.GetComponent<BoxCollider>();
            if (boxCol != null)
            {
                Vector3 worldCenter = previewInstance.transform.TransformPoint(boxCol.center);
                Vector3 halfExtents = Vector3.Scale(boxCol.size, previewInstance.transform.lossyScale) * 0.5f;

                Gizmos.color = Color.red;
                Gizmos.matrix = Matrix4x4.TRS(worldCenter, previewInstance.transform.rotation, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, halfExtents * 2);
            }
        }
    }

    //-----------------------------------------------------Active/Desactive des systèmes ---
    public void ToggleSnapRotation()
    {
        snapRotationEnabled = !snapRotationEnabled;
        if (snapRotationEnabled)
        {
            targetRotationY = Mathf.Round(targetRotationY / snapAngle) * snapAngle;
        }
    }

}
