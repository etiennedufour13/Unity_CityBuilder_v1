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
            Debug.LogError("MaterialController introuvable sur le prefab sélectionné !");
        }
    }

    void UpdatePreviewPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            previewInstance.transform.position = hit.point;
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
        if (Input.GetKey(KeyCode.R) && !Input.GetKey(KeyCode.LeftShift))
        {
            rotationY += 100f * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.LeftShift))
        {
            rotationY += -100f * Time.deltaTime;
        }
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
}
