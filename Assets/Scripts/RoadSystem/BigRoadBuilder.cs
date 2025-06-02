using SplineMeshTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class BigRoadBuilder : MonoBehaviour
{
    public GameObject roadPointPrefab;
    public float pointDistance = 2f;
    public float maxAngle = 90f;
    private bool roadBuildingActive = false;
    private bool rigidMode = false;
    public List<GameObject> roadPoints = new List<GameObject>();
    public GameObject roadSegmentPrefab;
    public GameObject temporairePrefab;
    private SplineContainer splineContainer;
    private LineRenderer lineRenderer;

    void Start()
    {
        splineContainer = gameObject.AddComponent<SplineContainer>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        if (!roadBuildingActive) return;

        if (roadPoints.Count > 0)
        {
            UpdateActivePoint();
        }

        if (Input.GetMouseButtonDown(0))
        {
            CreateRoadPoint();
        }

        if (Input.GetMouseButtonDown(1))
        {
            CancelLastPoint();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidMode = !rigidMode;
        }
    }

    public void ToggleRoadBuilding()
    {
        roadBuildingActive = !roadBuildingActive;
        lineRenderer.enabled = roadBuildingActive;
    }

    private void CreateSpine()
    {

        Spline spline = splineContainer.Splines[0]; //on parle de quelle spline ? (yen a qu'une)
        float tension = 0.4f; // distance des leviers de tangente vis à vis de leurs points

        //retire les derniers points (qui sont actifs et suivent la souris
        if (spline.Knots.Count() == roadPoints.Count)
        {
            if (roadPoints.Count == 2) //retire un knot de plus
            {
                spline.Remove(spline.Knots.ElementAt(spline.Knots.Count() - 1));
                spline.Remove(spline.Knots.ElementAt(spline.Knots.Count() - 1));
            }
            else if (roadPoints.Count > 2)
            {
                spline.Remove(spline.Knots.ElementAt(spline.Knots.Count() - 1));
            }
        }

        //initialisation (1er clic)
        if (roadPoints.Count == 2)
        {
            // Point initial (0)
            Vector3 posRoadPointZero = roadPoints[0].transform.position;
            spline.Add(new BezierKnot(posRoadPointZero));

            // Point suivant (1)
            Vector3 posRoadPointUn = roadPoints[1].transform.position;
            Vector3 dir = (posRoadPointUn - posRoadPointZero).normalized;
            float dist = Vector3.Distance(posRoadPointZero, posRoadPointUn) * tension;

            // Tangente entrante orientée vers posRoadPointZero
            Vector3 tangentIn = -dir * dist * 0.5f;
            // Tangente sortante orientée dans la continuité de la droite
            Vector3 tangentOut = dir * dist * 0.5f;

            spline.Add(new BezierKnot(posRoadPointUn, tangentIn, tangentOut));

        }
        else if (roadPoints.Count > 2)//les clics d'après
        {
            Vector3 pointPrecedent = roadPoints[roadPoints.Count - 2].transform.position;
            Vector3 tangentePointPrecedent = spline.Knots.ElementAt(roadPoints.Count - 2).TangentOut;
            Vector3 pointActuel = roadPoints[roadPoints.Count -1].transform.position;

            // Calcul de la direction entre les points
            Vector3 dirPrecedentActuel = (pointActuel - pointPrecedent).normalized;

            // Calcul de la distance pondérée
            float dist = Vector3.Distance(pointPrecedent, pointActuel) * tension;

            // Calcul de l'angle alpha entre la tangente du point précédent et la droite reliant les deux points
            float alpha = Vector3.SignedAngle(tangentePointPrecedent, pointPrecedent, pointActuel);
            Quaternion rotation = Quaternion.AngleAxis(-alpha, Vector3.up); // Inversion pour respecter l’orientation demandée

            // Définition de la tangente entrante du point actuel
            Vector3 newTangentIn = rotation * (dirPrecedentActuel * dist * 0.5f);

            // Définition de la tangente sortante comme l’opposée de la tangente entrante
            Vector3 newTangentOut = new Vector3(-newTangentIn.x, newTangentIn.y, -newTangentIn.z);

            // Ajout du nouveau point à la spline
            spline.Add(new BezierKnot(pointActuel, newTangentIn, newTangentOut));


        }




        UpdateLineRenderer();
    }

    private void UpdateLineRenderer()
    {
        if (splineContainer.Splines.Count == 0) return;

        Spline spline = splineContainer.Splines[0];
        int resolution = 20;
        List<Vector3> linePoints = new List<Vector3>();

        for (int i = 0; i < spline.Knots.Count() - 1; i++) // Correction de .Count en .Count()
        {
            BezierKnot p0 = spline.Knots.ElementAt(i);   // Utilisation de ElementAt() pour accéder aux éléments
            BezierKnot p1 = spline.Knots.ElementAt(i + 1);


            for (int j = 0; j <= resolution; j++)
            {
                float t = j / (float)resolution;
                Vector3 point = BezierPoint(p0.Position, p0.Position + p0.TangentOut, p1.Position + p1.TangentIn, p1.Position, t);
                linePoints.Add(point);
            }
        }

        lineRenderer.positionCount = linePoints.Count;
        lineRenderer.SetPositions(linePoints.ToArray());
    }

    private Vector3 BezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        float uu = u * u;
        float uuu = uu * u;
        float tt = t * t;
        float ttt = tt * t;

        return (uuu * p0) + (3 * uu * t * p1) + (3 * u * tt * p2) + (ttt * p3);
    }

    private void CreateRoadPoint()
    {
        Vector3 position = GetMouseWorldPosition();
        if (roadPoints.Count == 0)
        {
            CreatePoint(position);
        }
        CreatePoint(position);
    }

    private void CreatePoint(Vector3 position)
    {
        GameObject newPoint = Instantiate(roadPointPrefab, new Vector3 (position.x, position.y + 0.01f, position.z), Quaternion.identity, transform);
        roadPoints.Add(newPoint);
        Debug.Log(roadPoints.Count);
        CreateSpine();
    }

    private void UpdateActivePoint()
    {
        if (roadPoints.Count < 2) return;

        GameObject activePoint = roadPoints[roadPoints.Count - 1];
        GameObject previousPoint = roadPoints[roadPoints.Count - 2];

        Vector3 newPos;

        if (rigidMode && roadPoints.Count > 2)
        {
            GameObject secondPreviousPoint = roadPoints[roadPoints.Count - 3];
            Vector3 direction = (previousPoint.transform.position - secondPreviousPoint.transform.position).normalized;
            newPos = previousPoint.transform.position + direction * pointDistance;
        }
        else
        {
            Vector3 mousePos = GetMouseWorldPosition();
            Vector3 direction = (mousePos - previousPoint.transform.position).normalized;
            newPos = previousPoint.transform.position + direction * pointDistance;
        }

        if (roadPoints.Count > 2)
        {
            GameObject secondPreviousPoint = roadPoints[roadPoints.Count - 3];
            if (!IsAngleValid(secondPreviousPoint.transform.position, previousPoint.transform.position, newPos))
            {
                return;
            }
        }

        activePoint.transform.position = newPos;
        CreateSpine();
    }

    private bool IsAngleValid(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 dir1 = (p2 - p1).normalized;
        Vector3 dir2 = (p3 - p2).normalized;
        float angle = Vector3.Angle(dir1, dir2);
        return angle <= maxAngle;
    }

    private void CancelLastPoint()
    {
        if (roadPoints.Count == 0) return;

        GameObject lastPoint = roadPoints[roadPoints.Count - 1];
        roadPoints.RemoveAt(roadPoints.Count - 1);
        Destroy(lastPoint);

        if (roadPoints.Count < 2)
        {
            roadPoints.Clear();
            roadBuildingActive = false;
            lineRenderer.enabled = false;
            return;
        }

        GameObject roadSegment = Instantiate(roadSegmentPrefab, Vector3.zero, Quaternion.identity);
        foreach (var point in roadPoints)
        {
            point.transform.parent = roadSegment.transform;
        }

        roadPoints.Clear();
        roadBuildingActive = false;
        lineRenderer.enabled = false;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
