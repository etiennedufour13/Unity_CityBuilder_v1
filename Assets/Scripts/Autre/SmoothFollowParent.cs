using UnityEngine;

public class SmoothFollowParent : MonoBehaviour
{
    public float smoothTime = 0.3f;
    private Vector3 velocity = Vector3.zero;
    private Vector3 targetWorldPos;

    void Start()
    {
        targetWorldPos = transform.position;
    }

    void LateUpdate()
    {
        targetWorldPos = Vector3.SmoothDamp(
            targetWorldPos,
            transform.parent.position,
            ref velocity,
            smoothTime
        );

        transform.position = targetWorldPos;
    }
}
