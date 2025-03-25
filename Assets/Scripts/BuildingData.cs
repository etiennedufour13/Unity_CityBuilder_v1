using UnityEngine;

[CreateAssetMenu(fileName = "New Building", menuName = "Building")]
public class BuildingData : ScriptableObject
{
    public GameObject prefab;
    public string buildingName;

    public int gridWidth = 1;
    public int gridLength = 1;
}

