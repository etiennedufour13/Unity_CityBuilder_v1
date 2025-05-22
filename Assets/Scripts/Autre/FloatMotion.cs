using UnityEngine;

public class FloatMotion : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.5f;
    public float minRange = 0.1f;
    public float maxRange = 0.5f;

    private float speed;
    private float range;
    private Vector3 localStartPos;

    public bool isActive = true;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);
        range = Random.Range(minRange, maxRange);
        localStartPos = transform.localPosition;
    }

    void Update()
    {
        if (isActive){
            float offset = Mathf.Sin(Time.time * speed) * range;
            transform.localPosition = localStartPos + new Vector3(0f, offset, 0f);
        }        
    }
}
