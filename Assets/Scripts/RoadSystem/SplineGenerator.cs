using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;
using SplineMeshTools.Core;

public class SplineGenerator : MonoBehaviour
{
    public SplineContainer splineContainer;
    public float tension = 0.4f; // Ajuste la souplesse des courbes

    public void GenerateSpline(Spline referenceSpline)
    {
        Spline spline = splineContainer.Spline;
        spline.Clear();

        foreach (var knot in referenceSpline)
            spline.Add(knot);

        //splineContainer.Refresh();
        GetComponent<SplineMeshTools.Core.SplineMesh>().GenerateMeshAlongSpline();
    }
}
