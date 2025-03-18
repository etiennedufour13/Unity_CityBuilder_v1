using UnityEngine;

public class CloudShadowController : MonoBehaviour
{
    public Material cloudMaterial;
    public float speed = 0.05f;

    void Update()
    {
        if (cloudMaterial)
        {
            cloudMaterial.SetFloat("_Speed", speed);
        }
    }
}
