using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    public float gridSize = 1f;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public Material gridMaterial;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = gridMaterial;

        GenerateGrid();
    }

    void GenerateGrid()
    {
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[(gridWidth + 1) * (gridHeight + 1)];
        int[] indices = new int[gridWidth * (gridHeight + 1) * 2 + gridHeight * (gridWidth + 1) * 2];

        int v = 0, i = 0;
        for (int x = 0; x <= gridWidth; x++)
        {
            for (int z = 0; z <= gridHeight; z++)
            {
                vertices[v++] = new Vector3((x - gridWidth / 2f) * gridSize, 0, (z - gridHeight / 2f) * gridSize);
            }
        }

        v = 0;
        for (int x = 0; x <= gridWidth; x++)
        {
            indices[i++] = v;
            indices[i++] = v + gridHeight;
            v += (gridHeight + 1);
        }

        v = 0;
        for (int z = 0; z <= gridHeight; z++)
        {
            indices[i++] = v;
            indices[i++] = v + gridWidth * (gridHeight + 1);
            v++;
        }

        mesh.vertices = vertices;
        mesh.SetIndices(indices, MeshTopology.Lines, 0);
        meshFilter.mesh = mesh;
    }
}

