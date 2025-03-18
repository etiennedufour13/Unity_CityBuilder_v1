using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Splines;

public class RoadTest : MonoBehaviour
{
    public GameObject roadPointPrefab;
    public float pointDistance = 2f;
    public float maxAngle = 90f;
    public float tension = 0.4f;
    private bool roadBuildingActive = false;
    private bool rigidMode = false;
    public List<GameObject> roadPoints = new List<GameObject>();
    public GameObject roadSegmentPrefab;
    private SplineContainer splineContainer;


    //-------------------------------------------------------------- GESTION ---
    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    //gère les inputs et les voids actives
    void Update()
    {
        if (!roadBuildingActive) return;

        //met à jour le point actif si il y en a un
        if (roadPoints.Count > 0)
        {
            UpdateActivePoint();
        }

        //place un point
        if (Input.GetMouseButtonDown(0))
        {
            CreateRoadPoint();
        }

        //stop la construction
        if (Input.GetMouseButtonDown(1))
        {
            CancelLastPoint();
        }

        //mode rigide
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidMode = !rigidMode;
        }
    }

    //appelé par les UI, active/desactive la construction
    public void ToggleRoadBuilding()
    {
        roadBuildingActive = !roadBuildingActive;
        
    }


    //-------------------------------------------------------------- CREATION POINT ---
    //Créé un point
    private void CreateRoadPoint()
    {
        Vector3 position = GetMouseWorldPosition();

        //initialisation en cas de 1er clic
        if (roadPoints.Count == 0) CreatePoint(position);

        CreatePoint(position);
    }

    //Définit la position du point
    private void CreatePoint(Vector3 position)
    {
        GameObject newPoint = Instantiate(roadPointPrefab, new Vector3(position.x, position.y + 0.01f, position.z), Quaternion.identity, transform);
        roadPoints.Add(newPoint);
        CreateSpine();
    }

    //ajoute des nouveaux roadPoints à la spine
    private void CreateSpine()
    {
        if (roadPoints.Count < 2) return; //ne fait rien pour le point d'origine


        Spline spline = splineContainer.Splines[0]; //on parle de la spline 0

        if (roadPoints.Count == 2)
        {
            //simple création des points de la spline (sans s'occuper des béziers)
            Vector3 pos0 = roadPoints[0].transform.position;
            Vector3 pos1 = GetMouseWorldPosition();
            spline.Add(new BezierKnot(pos0, pos0, pos0));
            spline.Add(new BezierKnot(pos1, pos1, pos1));
        }
        else
        {
            Vector3 newPos = GetNextRoadPos();
            spline.Add(new BezierKnot(newPos, newPos, newPos));
        }
    }


    //-------------------------------------------------------------- MISE A JOUR DYNAMIQUE ---
    //update le spline et le roadPoints
    private void UpdateActivePoint()
    {
        if (roadPoints.Count < 2) return;

        //variables
        Spline spline = splineContainer.Splines[0];

        //quand il y a deux points : bouger les deux
        if (roadPoints.Count == 2){
            Vector3 pos0 = roadPoints[0].transform.position;
            Vector3 pos1 = GetMouseWorldPosition();

            //définition des roadPoints (pas besoin pour l'initial qui bouge pas)
            roadPoints[1].transform.position = pos1;

            //définition des splineKnots
            if (spline.Knots.Count() < 2) return; //vérification qu'ils sont bien créés
            spline.Clear();
            spline.Add(new BezierKnot(pos0, pos0, (pos1 - pos0).normalized * tension));
            spline.Add(new BezierKnot(pos1, (pos0 - pos1).normalized * tension, (pos1 - pos0).normalized * tension));

        }
        else
        {
            Debug.Log("3");
            //variables
            Vector3 newPos = GetNextRoadPos();

            //définition du roadPoint
            roadPoints[roadPoints.Count -1].transform.position = newPos;

            //définition des splineKnots
            if (spline.Knots.Count() < 3) return; //vérification qu'ils sont bien créés

            //variables
            BezierKnot lastKnot = spline[spline.Count - 2];
            Vector3 lastKnotPos = lastKnot.Position;
            Vector3 lastKnotTan = lastKnot.TangentOut;
            float angle = Vector3.SignedAngle(lastKnotTan, newPos - lastKnotPos, Vector3.up);
            Vector3 newTanIn = Quaternion.AngleAxis(angle, Vector3.up) * (lastKnotPos - newPos).normalized * tension;



            spline.RemoveAt(spline.Count - 1);
            spline.Add(new BezierKnot(newPos, newTanIn, -newTanIn));
        }
    }


    //-------------------------------------------------------------- FIN D'INTERACTION ---
    private void CancelLastPoint()
    {
        if (roadPoints.Count == 0) return;

        GameObject lastPoint = roadPoints[roadPoints.Count - 1];
        roadPoints.RemoveAt(roadPoints.Count - 1);
        Destroy(lastPoint);
        Spline spline = splineContainer.Splines[0];
        spline.RemoveAt(spline.Count - 1);

        if (roadPoints.Count < 2)
        {
            roadPoints.Clear();
            spline.Clear();
            roadBuildingActive = false;
            return;
        }

        GameObject roadSegment = Instantiate(roadSegmentPrefab, Vector3.zero, Quaternion.identity);
        foreach (var point in roadPoints)
        {
            point.transform.parent = roadSegment.transform;
            roadSegment.GetComponent<SplineGenerator>().GenerateSpline(spline);
        }

        roadPoints.Clear();
        roadBuildingActive = false;
    }


    //-------------------------------------------------------------- GESTION ---
    //position du Raycast et du raycast contraint
    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    private Vector3 GetNextRoadPos()
    {
        if (roadPoints.Count < 2) return GetMouseWorldPosition();

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

            }
        }

        return newPos;
    }

    private bool IsAngleValid(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 dir1 = (p2 - p1).normalized;
        Vector3 dir2 = (p3 - p2).normalized;
        float angle = Vector3.Angle(dir1, dir2);
        return angle <= maxAngle;
    }
}
