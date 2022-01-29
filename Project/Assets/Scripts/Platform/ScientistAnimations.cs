using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScientistAnimations : MonoBehaviour
{
    public float startDistance = 5;
    public float animDuration = 2;
    public bool visible = false;
    private float animTime = 0;
    public float lightConeAnimOffset = 0.8f;
    private Vector3 startPosition;
    public CameraController lightConeRenderer;
    public float breathSpeed = 0.3f;
    public float breathMovement = 2;
    private float breathTime = 0;
    public Transform eye;
    public float eyeAngleAnimSpeed = 0.5f;
    public float eyeMaxDistance = 0.5f;
    public Vector3 eyeStartOffset;

    public float blinkMinDuration = 1;
    public float blinkMaxDuration = 5;
    private float blinkTime = 5;
    private AnimatedSprite animatedSprite;
    private Vector3 appearAxis;
    private SpriteRenderer eyeSpriteRenderer;

    void Start()
    {
        startPosition = transform.position;
        eyeStartOffset = eye.transform.localPosition;
        animatedSprite = GetComponent<AnimatedSprite>();
        appearAxis = (transform.position - Camera.main.transform.position).normalized;
        eyeSpriteRenderer = animatedSprite.spriteRenderer.transform.GetChild(0).GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if(visible)
            animTime += Time.deltaTime;
        else animTime -= Time.deltaTime;
        breathTime += Time.deltaTime;
        animTime = Mathf.Clamp(animTime, 0, animDuration);
        float animRatio = (1 - animTime / animDuration);
        transform.position = startPosition + animRatio * animRatio * appearAxis * startDistance + Vector3.up * breathMovement * Mathf.Sin(breathSpeed * breathTime);
        lightConeRenderer.visibility = Mathf.Clamp01(animTime / animDuration - lightConeAnimOffset) / (1 - lightConeAnimOffset);
        float colorRatio = 1 - animRatio * animRatio;
        animatedSprite.spriteRenderer.color = new Color(colorRatio, colorRatio, colorRatio, 1);
        eyeSpriteRenderer.color = new Color(colorRatio, colorRatio, colorRatio, 1);
        eye.transform.localPosition = eyeStartOffset + Quaternion.AngleAxis(Mathf.Sin(Time.time * eyeAngleAnimSpeed) * lightConeRenderer.deltaAngle, Vector3.forward) * lightConeRenderer.startDirection * eyeMaxDistance;
        blinkTime -= Time.deltaTime;
        if(blinkTime < 0)
        {
            blinkTime += Random.Range(blinkMinDuration, blinkMaxDuration);
            animatedSprite.SelectAnim("Blink", false, true);
        }
    }
}
