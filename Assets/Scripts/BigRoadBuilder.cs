using System.Collections.Generic;
using UnityEngine;

public class BigRoadBuilder : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float pointSpacing = 1f;

    private List<Vector3> controlPoints = new List<Vector3>();

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
            lineRenderer.startWidth = 0.2f;
            lineRenderer.endWidth = 0.2f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 newPoint = GetMouseWorldPosition();
            if (controlPoints.Count == 0 || Vector3.Distance(newPoint, controlPoints[controlPoints.Count - 1]) > pointSpacing)
            {
                controlPoints.Add(newPoint);
                UpdateSpline();
            }
        }
        else if (Input.GetMouseButtonDown(1)) 
        {
            ClearSpline();
        }
    }

    private void UpdateSpline()
    {
        if (controlPoints.Count < 2) return;

        List<Vector3> splinePoints = GenerateSpline(controlPoints, 10);
        lineRenderer.positionCount = splinePoints.Count;
        lineRenderer.SetPositions(splinePoints.ToArray());
    }

    private void ClearSpline()
    {
        controlPoints.Clear();
        lineRenderer.positionCount = 0;
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

    private List<Vector3> GenerateSpline(List<Vector3> points, int resolution)
    {
        List<Vector3> spline = new List<Vector3>();
        for (int i = 0; i < points.Count - 1; i++)
        {
            Vector3 p0 = i > 0 ? points[i - 1] : points[i];
            Vector3 p1 = points[i];
            Vector3 p2 = points[i + 1];
            Vector3 p3 = i < points.Count - 2 ? points[i + 2] : points[i + 1];

            for (int j = 0; j <= resolution; j++)
            {
                float t = j / (float)resolution;
                spline.Add(CatmullRom(p0, p1, p2, p3, t));
            }
        }
        return spline;
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2 * p1) +
            (-p0 + p2) * t +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * t2 +
            (-p0 + 3 * p1 - 3 * p2 + p3) * t3
        );
    }
}
