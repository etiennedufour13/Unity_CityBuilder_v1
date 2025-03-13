using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMesh : MonoBehaviour
{
    public List<GameObject> roadPoints = new List<GameObject>();
    public float roadWidth = 1f; // Largeur de la route

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            CreateRoadMeshPoint();
            CreateRoadMesh();
        }
    }

    void CreateRoadMeshPoint()
    {
        if (roadPoints.Count == 0)
        {
            Debug.LogWarning("No road points available to create mesh points.");
            return;
        }

        for (int i = 0; i < roadPoints.Count; i++)
        {
            Vector3 roadPoint = roadPoints[i].transform.position;

            if (i == 0)
            {
                // Pas de point précédent pour le premier point
                CreateEmptyGameObject("RoadPoint_Left_" + i, roadPoint);
                CreateEmptyGameObject("RoadPoint_Right_" + i, roadPoint);
            }
            else
            {
                Vector3 previousPoint = roadPoints[i - 1].transform.position;
                Vector3 direction = (roadPoint - previousPoint).normalized;

                // Calculer les points à 90 degrés et -90 degrés
                Vector3 leftPoint = roadPoint + Quaternion.Euler(0, 90, 0) * direction * roadWidth / 2;
                Vector3 rightPoint = roadPoint + Quaternion.Euler(0, -90, 0) * direction * roadWidth / 2;

                CreateEmptyGameObject("RoadPoint_Left_" + i, leftPoint);
                CreateEmptyGameObject("RoadPoint_Right_" + i, rightPoint);
            }
        }
    }

    void CreateRoadMesh()
    {
        // Implémentation de la création du mesh de la route
    }

    private GameObject CreateEmptyGameObject(string name, Vector3 position)
    {
        GameObject newPoint = new GameObject(name);
        newPoint.transform.position = position;
        newPoint.transform.parent = transform;
        return newPoint;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (var roadPoint in roadPoints)
        {
            Gizmos.DrawSphere(roadPoint.transform.position, 0.1f);
        }
    }
}
