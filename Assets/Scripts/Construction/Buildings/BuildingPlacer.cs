using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BuildingPlacer : MonoBehaviour
{
    //autre
    private Camera cam;
    public Color obstructColor;

    //layers et tags
    public LayerMask groundLayer;
    public List<string> forbiddenTags = new List<string> { "Building", "Path" };
    public List<string> vegetauxTags = new List<string> { };

    //utiles dans le code
    private BuildingData selectedBuilding;
    private bool isValidPlacement = false;
    private MaterialController materialController;
    private HashSet<GameObject> overlappingObjects = new HashSet<GameObject>();
    private float rotationY = 0f;
    float targetRotationY; // pour stocket la rotation de manière temporaire
    int buildingWidth, buildingLength; // utilisé pour la grille
    private bool waitForRotation; //bool qui stock que l'ont veut une grid mais pas le temps de la rotation (sinon c'est dégeu)
    private GridVisualizer gridVisualizer;
    public GameObject previewInstance;

    //contraintes de placement
    private bool snapRotationEnabled = false; //système de crantage
    public float snapAngle = 30f; //degré de crantage
    public bool gridPlacementEnabled;
    private GameObject snappedTarget = null; // autre batiment avec lequel il se snap

    //paramétrage libre
    public float gridSize = 1f; //taille de la grille
    float rotationSpeed = 500f; // Vitesse en degrés par seconde





    //------------------------------------------------------------------------------ Gestion ---
    void Start()
    {
        gridVisualizer = FindObjectOfType<GridVisualizer>();
        cam = Camera.main;
    }

    void Update()
    {
        if (previewInstance != null)
        {
            UpdatePreviewPosition();
            CheckOverlapingPlacement();
            HandleRotation();
            HandlePlacement();
        }
    }


    //------------------------------------------------------------------------------ Mise en place de la preview ---
    public void SelectBuilding(BuildingData building)
    {
        //setup de la nouvelle preview
        if (previewInstance != null) Destroy(previewInstance);
        selectedBuilding = building;
        previewInstance = Instantiate(building.prefab);

        //changement du layermask et du tag pour ne pas capter le raycast ni les effets de bat
        previewInstance.layer = LayerMask.NameToLayer("Ignore Raycast");
        previewInstance.tag = "PreviewBuilding";
        foreach (Transform child in transform) child.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        //ajout d'un ridgidbody à la preview
        Rigidbody rb = previewInstance.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        //récupération de la taille du bat
        buildingWidth = selectedBuilding.gridWidth;
        buildingLength = selectedBuilding.gridLength;

        //activation du trigger pour le collider
        Collider previewCollider = previewInstance.GetComponent<Collider>();
        if (previewCollider != null) { previewCollider.isTrigger = true; }

        //lancement de la coroutine qui va lancer le système de matériau dynamique
        StartCoroutine(InitializeMaterialController());
    }

    private IEnumerator InitializeMaterialController() //coroutine pour faire un délais évitant des bugs
    {
        yield return null;
        materialController = previewInstance.GetComponentInChildren<MaterialController>();
        if (materialController != null){ materialController.SetTransparent(true); }
    }


    //------------------------------------------------------------------------------ Update en cours de placement ---
    private void UpdatePreviewPosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;

            //conditionnement du placement par la grille
            if (gridPlacementEnabled)
            {
                Quaternion rotation = Quaternion.Euler(0, targetRotationY, 0);
                Vector3 localPosition = Quaternion.Inverse(rotation) * targetPosition;

                float snappedX = Mathf.Round(localPosition.x / gridSize) * gridSize;
                float snappedZ = Mathf.Round(localPosition.z / gridSize) * gridSize;

                snappedX += ((buildingWidth % 2) == 0) ? gridSize / 2f : 0;
                snappedZ += ((buildingLength % 2) == 0) ? gridSize / 2f : 0;

                if (!waitForRotation)
                    targetPosition = rotation * new Vector3(snappedX, localPosition.y, snappedZ);

                gridVisualizer.DrawGrid(targetPosition, gridSize, targetRotationY);
            }

            //définition de la position et rotation
            previewInstance.transform.position = targetPosition;
            previewInstance.transform.rotation = Quaternion.Euler(0, rotationY, 0);

            // ---------- SNAP SYSTEM ----------
            Building buildingComponent = previewInstance.GetComponent<Building>();
            if (buildingComponent != null && buildingComponent.canBeSnap)
            {
                Transform myLeft = previewInstance.transform.Find("SnapPointLeft");
                Transform myRight = previewInstance.transform.Find("SnapPointRight");
                if (myLeft == null || myRight == null) return;

                float snapRange = 1f;
                Transform targetPoint = null;
                Transform mySnap = null;
                float closestDistance = Mathf.Infinity;

                Collider[] hitColliders = Physics.OverlapBox(
                    previewInstance.transform.position,
                    previewInstance.GetComponent<Collider>().bounds.extents,
                    previewInstance.transform.rotation);

                foreach (var col in hitColliders)
                {
                    if (col.gameObject == previewInstance) continue;

                    Building other = col.GetComponent<Building>();
                    if (other != null && other.canBeSnap)
                    {
                        Transform otherLeft = other.transform.Find("SnapPointLeft");
                        Transform otherRight = other.transform.Find("SnapPointRight");
                        if (otherLeft == null || otherRight == null) continue;

                        float distLeftRight = Vector3.Distance(myLeft.position, otherRight.position);
                        float distRightLeft = Vector3.Distance(myRight.position, otherLeft.position);

                        if (distLeftRight < snapRange && distLeftRight < closestDistance)
                        {
                            closestDistance = distLeftRight;
                            mySnap = myLeft;
                            targetPoint = otherRight;
                        }

                        if (distRightLeft < snapRange && distRightLeft < closestDistance)
                        {
                            closestDistance = distRightLeft;
                            mySnap = myRight;
                            targetPoint = otherLeft;
                        }
                    }
                }

                if (targetPoint != null && mySnap != null)
                {
                    Vector3 offset = mySnap.position - previewInstance.transform.position;
                    previewInstance.transform.position = targetPoint.position - offset;
                }
                else
                {
                    previewInstance.transform.position = targetPosition;
                }
            }




        }
    }

    private void CheckOverlapingPlacement()
    {
        //affichage des objets overlappés
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
        if (materialController != null) materialController.SetOutline(!isValidPlacement, obstructColor);
    }

    void HandleRotation()
    {
        //gestion des inputs
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

        //application de la rotation
        rotationY = Mathf.MoveTowardsAngle(rotationY, targetRotationY, rotationSpeed * Time.deltaTime);

        //désactive le crantage en cas de retard de rotation
        if (snapRotationEnabled){
            if (targetRotationY != rotationY){
                waitForRotation = true;
            }
            else {
                waitForRotation = false;
            }
        }
        else{
            if (Input.GetKey(KeyCode.R)){
                waitForRotation = true;
            }
            else {
                waitForRotation = false;
            }
        }
    }


    //------------------------------------------------------------------------------ Placement du bat ---
    void HandlePlacement()
    {
        if (!EventSystem.current.IsPointerOverGameObject()){
            if (Input.GetMouseButtonDown(0) && isValidPlacement) // placement
            {
                GameObject instance = Instantiate(selectedBuilding.prefab, previewInstance.transform.position, previewInstance.transform.rotation);          

                //supression des végétaux
                Collider box = previewInstance.GetComponent<Collider>();
                Collider[] colliders = Physics.OverlapBox(box.bounds.center, box.bounds.extents, box.transform.rotation);

                foreach (Collider col in colliders)
                {
                    if (vegetauxTags.Contains(col.tag))
                    {
                        Destroy(col.gameObject);
                    }
                }

                //suppression de la preview et de la grille
                Destroy(previewInstance);
                previewInstance = null;
                gridVisualizer.ClearGrid();

                //effet de placement
                BuildingData data = selectedBuilding;
                instance.GetComponent<Building>().PlacementEffects();
            }
            else if (Input.GetMouseButtonDown(1)) // annulation du placement
            {
                Destroy(previewInstance);
                previewInstance = null;
                gridVisualizer.ClearGrid();
            }
        }
    }


    //----------------------------------------------------- Active/Desactive des systèmes ---
    public void ToggleSnapRotation()
    {
        snapRotationEnabled = !snapRotationEnabled;
        if (snapRotationEnabled)
        {
            targetRotationY = Mathf.Round(targetRotationY / snapAngle) * snapAngle;
        }
    }

    public void ToggleGrid(){
        gridPlacementEnabled = !gridPlacementEnabled;
        if (!gridPlacementEnabled)
            gridVisualizer.ClearGrid();
    }


    //----------------------------------------------------- Autre ---
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
