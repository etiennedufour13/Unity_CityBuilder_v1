using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using LibTessDotNet;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class RoadBuilder : MonoBehaviour
{
    public Camera cam;
    public LayerMask groundLayer;
    public Material roadMaterial;

    private List<Vector3> points = new List<Vector3>();
    private LineRenderer lineRenderer;
    private LineRenderer pointRenderer;
    private bool isBuilding = false;
    private float closeDistanceThreshold = 0.2f; // Distance pour fermer la route

    //Dot de visualisation
    public GameObject visualPointPrefab;
    private GameObject visualPoint;

    //syst�me de v�rification du temps de clic droit
    private float currentClickTime;
    private bool clicDroit;
    public float maxClickTime;

    //snaping variables
    public float snapDetectionRadius = 3f;
    public float snapPointRadius = 0.2f;
    public GameObject snapPoint;
    public GameObject firstPoint;



    void Start()
    {
        //setup du syst�me de line
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.08f; //visuel
        lineRenderer.endWidth = 0.08f; //visuel
    }

    void Update()
    {
        if (!isBuilding) return;

        UpdateCurrentPointMarker();
        CheckInput();
        CheckSnapPoints();


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            visualPoint.SetActive(true);

            if (snapPoint != null)
            {
                visualPoint.transform.position = snapPoint.transform.position + Vector3.up * 0.01f;
            }
            else {
                visualPoint.transform.position = new Vector3 (hit.point.x, hit.point.y + 0.0002f, hit.point.z);
            }
        }
        else
        {
            visualPoint.SetActive(false);
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddPoint();
        }

        //clic droit (doit �tre rapide pour ne pas �tre confondu avec un d�placement)
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("entr�");
            clicDroit = true;
            currentClickTime = 0;
        }
        if (clicDroit)
        {
            currentClickTime += Time.deltaTime;
            if (currentClickTime > maxClickTime)
                clicDroit = false;
        }
        if(Input.GetMouseButtonUp(1) && clicDroit)
        {
            StartAndStopBuilding(false);
        }
    }

private void CheckSnapPoints()
{
    snapPoint = null; // Réinitialisation à chaque frame
    List<Transform> snapPoints = new List<Transform>();

    // Lancer un raycast depuis la souris
    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
    {
        Vector3 raycastPosition = hit.point;

        // Détection des objets "Path" autour de la position du raycast
        Collider[] colliders = Physics.OverlapSphere(raycastPosition, snapDetectionRadius);

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Path"))
            {
                Transform snapContainer = col.transform.Find("SnapPoints");
                if (snapContainer != null)
                {
                    foreach (Transform child in snapContainer)
                    {
                        snapPoints.Add(child);
                    }
                }
            }
        }

        // Ajout du premier point en cours de traçage comme snap point
        if (firstPoint != null)
        {
            snapPoints.Add(firstPoint.transform);
        }

        // Sélection du point le plus proche respectant le seuil
        if (snapPoints.Count > 0)
        {
            Transform closest = snapPoints.OrderBy(p => Vector3.Distance(p.position, raycastPosition)).First();
            if (Vector3.Distance(closest.position, raycastPosition) < snapPointRadius)
            {
                snapPoint = closest.gameObject;
            }
        }
    }
}



    public void StartAndStopBuilding(bool isStarting)
    {
        if (isStarting)
        {
            isBuilding = true;
            points.Clear();
            lineRenderer.positionCount = 0;
            visualPoint = Instantiate(visualPointPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            points.Clear();
            lineRenderer.positionCount = 0;
            Destroy(visualPoint);
            isBuilding = false;
            if (firstPoint != null)
            {
                Destroy(firstPoint);
                firstPoint = null;
            }
        }
    }

    private void AddPoint()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {

            Vector3 newPoint = (snapPoint != null) ? snapPoint.transform.position + Vector3.up * 0.01f : hit.point + Vector3.up * 0.01f;

            if (points.Count == 0)
            {
                firstPoint = new GameObject("FirstSnapPoint");
                firstPoint.transform.position = newPoint;
            }

            // V�rifie si le premier et dernier point sont proches pour fermer la route
            if (points.Count > 2 && Vector3.Distance(newPoint, points[0]) < closeDistanceThreshold)
            {
                CloseRoad();
                return;
            }

            points.Add(newPoint);
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }

    private void UpdateCurrentPointMarker()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 hoverPoint = hit.point + Vector3.up * 0.01f; // L�g�rement sur�lev�         

            if (points.Count > 0)
            {
                lineRenderer.positionCount = points.Count + 1;
                Vector3[] allPoints = new Vector3[points.Count + 1];
                points.CopyTo(allPoints);
                allPoints[points.Count] = (snapPoint != null) ? snapPoint.transform.position + Vector3.up * 0.01f : hoverPoint;
                lineRenderer.SetPositions(allPoints);
            }
        }
    }


    private void CloseRoad()
    {
        isBuilding = false;
        CreateRoadMesh();
    }

    private void CreateRoadMesh()
    {
        GameObject road = new GameObject("Road");
        road.tag = "Path";
        road.transform.position = Vector3.zero;
        MeshFilter meshFilter = road.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = road.AddComponent<MeshRenderer>();
        MeshCollider meshCollider = road.AddComponent<MeshCollider>();
        meshRenderer.material = roadMaterial;

        // Conversion des points en 2D
        ContourVertex[] contour = new ContourVertex[points.Count];
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 p = points[i];
            contour[i].Position = new Vec3 { X = p.x, Y = p.z, Z = 0 }; // On utilise XZ
        }

        // Triangulation
        Tess tess = new Tess();
        tess.AddContour(contour, ContourOrientation.Original);
        tess.Tessellate(WindingRule.EvenOdd, ElementType.Polygons, 3);

        // Construction du mesh Unity
        Vector3[] meshVertices = new Vector3[tess.Vertices.Length];
        for (int i = 0; i < tess.Vertices.Length; i++)
        {
            Vec3 v = tess.Vertices[i].Position;
            meshVertices[i] = new Vector3(v.X, points[0].y, v.Y); // Y fixe (XZ plane)
        }

        int[] meshTriangles = new int[tess.ElementCount * 3];
        for (int i = 0; i < tess.ElementCount; i++)
        {
            meshTriangles[i * 3] = tess.Elements[i * 3];
            meshTriangles[i * 3 + 1] = tess.Elements[i * 3 + 1];
            meshTriangles[i * 3 + 2] = tess.Elements[i * 3 + 2];
        }

        UnityEngine.Mesh mesh = new UnityEngine.Mesh();

        mesh.vertices = meshVertices;
        mesh.triangles = meshTriangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        road.AddComponent<MaterialController>();

        // Crée un GameObject enfant par sommet
        GameObject pointsContainer = new GameObject("SnapPoints");
        pointsContainer.transform.parent = road.transform;

        foreach (Vector3 point in mesh.vertices)
        {
            GameObject snapPoint = new GameObject("SnapPoint");
            snapPoint.transform.parent = pointsContainer.transform;
            snapPoint.transform.position = road.transform.TransformPoint(point);
        }

        //fin du code
        StartAndStopBuilding(false);
    }
}