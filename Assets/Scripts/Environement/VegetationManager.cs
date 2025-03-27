using UnityEngine;
using System.Collections.Generic;

public class VegetationManager : MonoBehaviour
{
    private static List<GameObject> vegetationInstances = new List<GameObject>();

    public static void RegisterVegetation(GameObject instance)
    {
        vegetationInstances.Add(instance);
    }

    public static void RemoveVegetationInArea(Vector3 center, float radius)
    {
        vegetationInstances.RemoveAll(obj =>
        {
            if (obj == null) return true;
            if (Vector3.Distance(obj.transform.position, center) < radius)
            {
                Destroy(obj);
                return true;
            }
            return false;
        });
    }
}
