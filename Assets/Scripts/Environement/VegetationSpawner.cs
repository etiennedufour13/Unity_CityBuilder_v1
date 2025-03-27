using UnityEngine;

public class VegetationSpawner : MonoBehaviour
{
    [System.Serializable]
    public class VegetationType
    {
        public GameObject prefab;
        public float minThreshold = 0.1f;
        public float maxThreshold = 0.9f;
    }

    public VegetationType grass;
    public VegetationType tree;

    public int gridSize = 50;
    public float noiseScale = 0.1f;
    public float patchSize = 2f;
    public float randomOffsetFactor = 0.5f;

    void Start()
    {
        GenerateVegetation();
    }

    void GenerateVegetation()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                float noiseValue = Mathf.PerlinNoise(x * noiseScale, z * noiseScale);

                TrySpawnVegetation(grass, noiseValue, x, z);
                TrySpawnVegetation(tree, noiseValue, x, z);
            }
        }
    }

    void TrySpawnVegetation(VegetationType vegetation, float noiseValue, int x, int z)
    {
        if (noiseValue < vegetation.minThreshold || noiseValue > vegetation.maxThreshold) return;

        float spawnChance = Mathf.InverseLerp(vegetation.minThreshold, vegetation.maxThreshold, noiseValue);
        if (Random.value > spawnChance) return;

        Vector3 pos = new Vector3(x * patchSize, 0, z * patchSize) + new Vector3(
            Random.Range(-randomOffsetFactor, randomOffsetFactor),
            0,
            Random.Range(-randomOffsetFactor, randomOffsetFactor)
        );

        GameObject instance = Instantiate(vegetation.prefab, pos, Quaternion.Euler(0, Random.Range(0, 360), 0));
        instance.transform.parent = transform;
    }
}
