using UnityEngine;

public class MaterialController : MonoBehaviour
{
    private Material material;
    private Color originalColor;

    void Start()
    {
        material = GetComponent<Renderer>().material;
        originalColor = material.color;
    }

    public void SetTransparent(bool transparent)
    {
        Color color = material.color;
        color.a = transparent ? 0.5f : 1f;
        material.color = color;
    }

    public void SetCollisionState(bool inCollision)
    {
        material.color = inCollision ? Color.red : originalColor;
    }
}
