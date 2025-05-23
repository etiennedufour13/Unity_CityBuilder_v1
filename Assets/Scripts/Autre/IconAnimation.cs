using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAnimation : MonoBehaviour
{
    private float moveHeight = 1f;
    public float minMoveHeight, maxMoveHeight;
    private float moveSpeed = 5f;
    public float minMoveSpeed, maxMoveSpeed;
    public float scaleSpeed = 5f;
    private float fadeSpeed = 5f;
    public float minFadeSpeed, maxFadeSpeed;
    public float minWaitTime, maxWaitTime;
    private float waitTime;

    private Vector3 targetLocalPosition;
    private Vector3 targetScale;
    private float targetOpacity;

    private Vector3 velocityPosition;
    private Vector3 velocityScale;
    private float currentOpacity;
    private float opacityVelocity;

    private SpriteRenderer sr;

    public FloatMotion floatMotion;
    private bool isActive;

    void Start()
    {
        moveHeight = Random.Range(minMoveHeight, maxMoveHeight);
        moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
        fadeSpeed = Random.Range(minFadeSpeed, maxFadeSpeed);
        waitTime = Random.Range(minWaitTime, maxWaitTime);

        sr = GetComponent<SpriteRenderer>();
        targetLocalPosition = transform.localPosition;
        targetScale = transform.localScale;
        targetOpacity = 1f;
        currentOpacity = sr != null ? sr.color.a : 1f;
    }

    IEnumerator Animate()
    {
        targetScale *= 1.2f;
        yield return new WaitForSeconds(0.15f);
        targetScale *= .8f;
         //pour laisser le temps à waitTime de se définir
        yield return new WaitForSeconds(waitTime);
        Debug.Log("Il s'est passé " +  waitTime);

        targetLocalPosition += new Vector3(0, moveHeight, -0.3f*moveHeight);
        yield return new WaitForSeconds(.2f);

        targetScale *= 1.5f;
        yield return new WaitForSeconds(.5f);

        targetOpacity = 0f;
    }

    void Update()
    {
        if (isActive){
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetLocalPosition, ref velocityPosition, 1f / moveSpeed);
            transform.localScale = Vector3.SmoothDamp(transform.localScale, targetScale, ref velocityScale, 1f / scaleSpeed);

            if (sr != null)
            {
                currentOpacity = Mathf.SmoothDamp(currentOpacity, targetOpacity, ref opacityVelocity, 1f / fadeSpeed);
                Color c = sr.color;
                c.a = currentOpacity;
                sr.color = c;
            }
        }
    }

    public void StartAnimation(){
        floatMotion.isActive = false;
        isActive = true;

        StartCoroutine(Animate());
    }
}
