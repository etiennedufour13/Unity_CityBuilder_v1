using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboarding : MonoBehaviour
{
    public Vector3 offSet;

    void Update() {
        // Oriente l'objet pour qu'il soit toujours face à la caméra
        transform.rotation = Camera.main.transform.rotation * Quaternion.Euler(offSet);
    }
}
