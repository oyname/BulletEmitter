using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private AnimationCurve movementCurve;
    private AnimationCurve rotationCurve;
    private float initialSpeed = 0f;
    private float currentSpeed = 0f;
    private float startTime = 0f;
    private Vector3 scaleBullet = default;
    private Vector3 positionBullet = default;
    private float rotationSpeed = 0f;
    private bool scale = false;
    private Color bulletColor;

    // Zeit nach dem sich das Geschoss auf das Ziel ausrichtet
    private float timeToAlignment = 0f;
    // Dauer wie lange das Ziel angeflogen wird
    private float duration = 0f;
    // Dauer der Rotation in Sekunden
    private float rotationDuration = 0f;
    // Soll sich das Geschoss auf das Ziel ausrichten?
    private bool align = false;

    private GameObject target;
    private bool isActionActive;
    private float timeSinceStart;
    private float currentAngle;
    private bool isRotating;
    private float rotationStartTime;
    private Renderer _renderer;

    public GameObject Target
    {
        set { target = value; }
    }
    public AnimationCurve RotationCurve
    {
        set { rotationCurve = value; }
    }
    public AnimationCurve MovementCurve
    {
        set { movementCurve = value; }
    }
    public bool Scale
    {
        set { scale = value; }
    }
    public Vector3 ScaleBullet
    {
        set { scaleBullet = value; }
    }
    public Vector3 PositionBullet
    {
        get { return positionBullet; }
        set { positionBullet = value; }
    }
    public float RotationSpeed
    {
        set { rotationSpeed = value; }
    }
    public float InitialSpeed
    {
        set { initialSpeed = value; currentSpeed = value; }
    }
    public float TimeToAlignment
    {
        set { timeToAlignment = value; }
    }
    public float Duration
    {
        set { duration = value; }
    }
    public bool Align
    {
        set { align = value; }
    }
    public float RotationDuration
    {
        set { rotationDuration = value; }
    }

    public Color BulletColor
    {
        get { return bulletColor; }
        set { bulletColor = value; }
    }

    private void Update()
    {
        timeSinceStart = Time.time - startTime;
        MoveBullet();
    }

    private void MoveBullet()
    {
        float movementValue = movementCurve.Evaluate(timeSinceStart);
        float rotationValue = rotationCurve.Evaluate(timeSinceStart);

        if (isActionActive)
        {
            AligntoTarget(TargetDirectionCalculate(target.transform));
        }
        else
        {
            // Rotate Object
            transform.Rotate(0f, 0f, rotationSpeed * rotationValue * Time.fixedDeltaTime);
        }

        // Move the object in the current direction
        transform.Translate(Vector3.right * currentSpeed * movementValue * Time.fixedDeltaTime);

        // Scale
        if (scale)
            transform.localScale = new Vector3(scaleBullet.x * Messenger.Instance.ScaleValue, scaleBullet.y * Messenger.Instance.ScaleValue, scaleBullet.z * Messenger.Instance.ScaleValue);
        else
            transform.localScale = new Vector3(scaleBullet.x, scaleBullet.y, scaleBullet.z);
    }

    private void Awake()
    {
        _renderer = this.gameObject.transform.GetComponent<Renderer>();
    }

    public void OnEnable()
    {
        bulletColor = new Color(bulletColor.r, bulletColor.g, bulletColor.b, 1.0f);
        if (align)
            Invoke("ActivateAction", timeToAlignment);
        rotationStartTime = Time.time;
        isRotating = true;
        currentSpeed = initialSpeed; // Set the initial velocity
        transform.Rotate(0f, 0f, 0f);
        startTime = Time.time;
        ChangeColor();
    }

    private void ActivateAction()
    {
        isActionActive = true;
        Invoke("DeactivateAction", duration);
    }

    private void DeactivateAction()
    {
        isActionActive = false;
    }

    private void AligntoTarget(float targetAngle)
    {
        // Aligns the object around the Z-axis to the target
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    private float TargetDirectionCalculate(Transform trans)
    {
        // Calculate the direction to the target position
        Vector3 directionToTarget = trans.position - transform.position;

        // Calculate the angle around the Z axis
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        if (isRotating)
        {
            float elapsedTime = Time.time - rotationStartTime;
            float progress = elapsedTime / rotationDuration;
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, progress);
            //Debug.Log("TEST: " + progress);
        }
        else
        {
            currentAngle = targetAngle;
        }

        return currentAngle;
    }
    public void ChangeColor()
    {
        _renderer.material.SetColor("_Color", bulletColor);
        _renderer.material.SetColor("_EmissionColor", bulletColor);
    }
}
