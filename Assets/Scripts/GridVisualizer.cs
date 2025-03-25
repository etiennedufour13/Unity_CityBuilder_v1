using UnityEngine;
using System.Collections.Generic;

public class GridVisualizer : MonoBehaviour
{
    public Material gridMaterial;
    public int gridRange = 5; // Distance autour du bâtiment
    public float fadeDistance = 5f; // Distance sur laquelle la grille s'estompe

    private List<GameObject> gridLines = new List<GameObject>();

    public void DrawGrid(Vector3 center, float gridSize, float rotationY)
    {
        ClearGrid();
        Quaternion rotation = Quaternion.Euler(0, rotationY, 0);

        for (int x = -gridRange; x <= gridRange; x++)
        {
            for (int z = -gridRange; z <= gridRange; z++)
            {
                Vector3 localStart = new Vector3(x * gridSize, 0, -gridRange * gridSize);
                Vector3 localEnd = new Vector3(x * gridSize, 0, gridRange * gridSize);
                CreateGridLine(rotation * localStart + center, rotation * localEnd + center, center);

                localStart = new Vector3(-gridRange * gridSize, 0, z * gridSize);
                localEnd = new Vector3(gridRange * gridSize, 0, z * gridSize);
                CreateGridLine(rotation * localStart + center, rotation * localEnd + center, center);
            }
        }
    }

    private void CreateGridLine(Vector3 start, Vector3 end, Vector3 center)
    {
        GameObject line = new GameObject("GridLine");
        LineRenderer lr = line.AddComponent<LineRenderer>();
        lr.material = gridMaterial;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });

        float distance = Vector3.Distance((start + end) / 2, center);
        float alpha = Mathf.Clamp01(1 - (distance / fadeDistance));
        Color lineColor = new Color(1f, 1f, 1f, alpha);
        lr.startColor = lineColor;
        lr.endColor = lineColor;

        gridLines.Add(line);
    }

    public void ClearGrid()
    {
        foreach (var line in gridLines)
        {
            Destroy(line);
        }
        gridLines.Clear();
    }
}
