using System.Collections.Generic;
using UnityEngine;

public class CurvePath : MonoBehaviour
{
    public List<Vector3> curvePoints = new List<Vector3>();

    public void SetPoints(List<Vector3> points)
    {
        curvePoints = new List<Vector3>(points);
    }

    public Vector3 GetPoint(float t)
    {
        if (curvePoints.Count < 2) return Vector3.zero;
        t = Mathf.Clamp01(t);
        int count = curvePoints.Count - 1;
        float scaledT = t * count;
        int index = Mathf.FloorToInt(scaledT);
        float lerpT = scaledT - index;

        if (index >= count) return curvePoints[count];

        return Vector3.Lerp(curvePoints[index], curvePoints[index + 1], lerpT);
    }
}
