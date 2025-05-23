using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using static UnityEngine.GraphicsBuffer;

public class BuildingDestroyer : MonoBehaviour
{
    private Camera cam;
    public LayerMask interactableLayer;
    public List<string> destructibleTags = new List<string> { "Building", "Path" };
    private bool destructionMode = false;
    private GameObject highlightedObject;
    private MaterialController materialController;


    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();
    }

    void Update()
    {
        if (!destructionMode) return;

        HandleHoverEffect();
        HandleInput();
    }

    public void ToggleDestructionMode()
    {
        destructionMode = !destructionMode;
        if (!destructionMode && highlightedObject != null)
        {
            ResetHighlight();
        }
    }

    private void HandleHoverEffect()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (destructibleTags.Contains(hit.transform.tag))
            {
                Debug.Log("capte un objet");
                GameObject target = hit.transform.gameObject;
                if (highlightedObject != target)
                {
                    Debug.Log("rend target");
                    ResetHighlight();
                    highlightedObject = target;
                    materialController = target.GetComponentInChildren<MaterialController>();
                    if (materialController != null)
                    {
                        Debug.Log("trouve le material controler");
                        materialController.SetCollisionState(true);
                    }
                }
            }
            else
            {
                ResetHighlight();
            }
        }
    }

    private void HandleInput()
    {
        if (!destructionMode) return;

        if (Input.GetMouseButtonDown(1)) // Clic droit -> désactiver
        {
            ToggleDestructionMode();
        }
        else if (Input.GetMouseButtonDown(0) && highlightedObject != null) // Clic gauche -> détruire
        {
            DestroyBuilding(highlightedObject);
        }
    }

    private void DestroyBuilding(GameObject building)
    {
        // Ici on pourra ajouter des effets de destruction, récupération de ressources, etc.
        Destroy(building);
    }

    private void ResetHighlight()
    {
        if (highlightedObject != null && materialController != null)
        {
            materialController.SetCollisionState(false);
        }
        highlightedObject = null;
        materialController = null;
    }
}
