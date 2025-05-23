using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zoomSpeed = 10f;
    public float rotationSpeed = 5f;
    public float minZoom = 5f;
    public float maxZoom = 100f;
    public float rotationCoef = 1f;
    public float zoomCoef = 1f;

    private Camera cam;
    private Vector3 pivot;
    private bool isRotating = false;
    private bool isPanning = false;
    private Vector3 panStartPoint;

    public LayerMask groundLayer;

    void Start()
    {
        cam = Camera.main;
        pivot = GetPivotPoint();
    }

    void Update()
    {
        HandleZoom();
        HandleRotation();
        HandlePanning();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            Vector3 zoomTarget = GetMousePivotPoint();
            Vector3 direction = (zoomTarget - cam.transform.position).normalized;
            float distance = Vector3.Distance(cam.transform.position, zoomTarget);
            float zoomAmount = scroll * zoomSpeed * distance * Time.deltaTime * zoomCoef;
            cam.transform.position += direction * zoomAmount;
            cam.transform.position = zoomTarget + (cam.transform.position - zoomTarget).normalized * Mathf.Clamp(Vector3.Distance(cam.transform.position, zoomTarget), minZoom, maxZoom);
        }
    }

void HandleRotation()
{
    if (Input.GetMouseButtonDown(2)) // Middle mouse button press
    {
        pivot = GetMousePivotPoint(); // Fixe le pivot une seule fois
        isRotating = true;
    }
    if (Input.GetMouseButton(2) && isRotating) // Middle mouse button held
    {
        float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * rotationCoef;
        float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * rotationCoef;
        
        cam.transform.RotateAround(pivot, Vector3.up, rotationX);
        cam.transform.RotateAround(pivot, cam.transform.right, -rotationY);
    }
    if (Input.GetMouseButtonUp(2)) // Middle mouse button release
    {
        isRotating = false;
    }
}




    void HandlePanning()
    {
        if (Input.GetMouseButtonDown(1)) // Right mouse button press
        {
            panStartPoint = GetMousePivotPoint();
            isPanning = true;
        }
        if (Input.GetMouseButton(1) && isPanning) // Right mouse button held
        {
            Vector3 currentPoint = GetMousePivotPoint();
            Vector3 offset = panStartPoint - currentPoint;
            cam.transform.position += offset;
            pivot += offset;
        }
        if (Input.GetMouseButtonUp(1)) // Right mouse button release
        {
            isPanning = false;
        }
    }

    Vector3 GetMousePivotPoint()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        return cam.transform.position + cam.transform.forward * 50f;
    }

    Vector3 GetPivotPoint()
    {
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            return hit.point;
        }
        return cam.transform.position + cam.transform.forward * 50f;
    }
}