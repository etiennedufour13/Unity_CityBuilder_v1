using System.Collections.Generic;
using UnityEngine;


public class BigRoadBuilder : MonoBehaviour
{
    public GameObject roadPointPrefab; // Prefab du point de route
    public float pointDistance = 2f; // Distance fixe entre les points
    public float maxAngle = 90f; // Angle maximum autorisé entre trois points
    private bool roadBuildingActive = false;
    private bool rigidMode = false;
    private List<GameObject> roadPoints = new List<GameObject>();
    


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
    }

    private void CreateRoadPoint()
    {
        Vector3 position = GetMouseWorldPosition();
        if (roadPoints.Count == 0)
        {
            CreatePoint(position); // Point initial
        }
        CreatePoint(position); // Nouveau point actif
    }

    private void CreatePoint(Vector3 position)
    {
        GameObject newPoint = Instantiate(roadPointPrefab, position, Quaternion.identity, transform);
        roadPoints.Add(newPoint);
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

        if (roadPoints.Count == 0)
        {
            roadBuildingActive = false;
            return;
        }
        
        if (roadPoints.Count == 1)
        {
            Destroy(roadPoints[0]);
            roadPoints.Clear();
            roadBuildingActive = false;
            return;
        }

        GameObject roadSegment = new GameObject("RoadSegment");
        roadSegment.transform.parent = transform;
        
        foreach (var point in roadPoints)
        {
            point.transform.parent = roadSegment.transform;
        }
        
        roadPoints.Clear();
        roadBuildingActive = false;
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
