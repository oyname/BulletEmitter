using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Revision 1.1

public class BulletEmitter : MonoBehaviour
{
    [SerializeField]
    private int bulletsAmount = 10;
    [SerializeField]
    private int NumberPerGroup = 1;
    [SerializeField]
    private float GroupAngle = 0;
    [SerializeField]
    private float repeatRate = 0.2f;
    [SerializeField]
    private int NumberOfRepetitions = 1;
    private int RepetitionsCnt = 0;
    [SerializeField]
    private float RepetitionsDelay = 0f;
    private float RepetitionsTimer = 0f;
    [SerializeField]
    float moveDistance = 0f;
    [SerializeField]
    [Range(0f, 360f)]
    private float startAngle = 90f, endAngle = 180f;
    [SerializeField]
    private bool scatter = false;
    [SerializeField]
    private GameObject target;
    [SerializeField]
    private bool toTarget = false;
    [SerializeField]
    private bool oscillate = false;
    [SerializeField]
    private float frequency = 2f;
    [SerializeField]
    private float oscillateAngle = 0f;
    [SerializeField]
    private float oscillateOpeningAngle = 0f;
    [SerializeField]
    private bool infinite = true;
    [SerializeField]
    private float rotateSpeed = 0f;
    [SerializeField]
    private float initialSpeed = 10f;
    [SerializeField]
    private AnimationCurve SpeedCurve;
    [SerializeField]
    private float BulletRotationSpeed;
    [SerializeField]
    private AnimationCurve BulletRotationSpeedCurve;
    [SerializeField]
    private Vector3 scaleBullet = new Vector3(1f, 1f, 1f);
    [SerializeField]
    private bool ActivateScalerCurve = false;
    [SerializeField]
    private AnimationCurve ScalerCurve;
    [SerializeField]
    private bool bulletAlign = false;
    [SerializeField]
    private float timeToAligment = 0f;
    [SerializeField]
    private float duration = 0f;
    [SerializeField]
    private float roationaDuration = 0f;
    [Tooltip("Color of BUllet")]
    public Color bulletColor;

    private Oscillation oscillator;

    // Activates the emitter
    private bool Activate;

    // Start Time
    private float startTime = 0f;

    private void OnDisable()
    {
        Activate = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        RepetitionsTimer = 0;
        Activate = false;
        oscillator = new Oscillation(frequency, oscillateAngle, oscillateOpeningAngle);

        Messenger.Instance.StartTime = Time.time;
        Messenger.Instance.ScaleCurve = ScalerCurve;
    }

    private void LateUpdate()
    {
        if (toTarget)
        {
            float targetAngle = TargetDirectionCalculate(target.transform);
            AligntoTarget(targetAngle);
            if (oscillate)
                oscillator.Angle = targetAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (RepetitionsTimer < Time.time && Activate)
        {
            // Starts the repeated action routine
            InvokeRepeating("RepeatedAction", 0f, repeatRate);
            Activate = false;
        }

        // Rotate the object around the Z - axis
        if (oscillate)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, oscillator.GetValue());
        }
        else
            transform.Rotate(0f, 0f, rotateSpeed * Time.fixedDeltaTime);
    }

    public void Fire()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        RepetitionsCnt = 0;
        RepetitionsTimer = 0;
        Activate = true;
        oscillator.SetParameters(frequency, oscillateAngle, oscillateOpeningAngle);
    }

    private float TargetDirectionCalculate(Transform trans)
    {
        // Calculate the direction to the target position
        Vector3 directionToTarget = trans.position - transform.position;

        // Calculate the angle around the Z axis
        float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

        return targetAngle;
    }

    private void AligntoTarget(float targetAngle)
    {
        // Aligns the object around the Z-axis to the target
        transform.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    }

    private void RepeatedAction()
    {
        float angleStep = (endAngle - startAngle) / (bulletsAmount - 1);
        float angle = startAngle;
        float angleDiff = endAngle - startAngle;
        float angle2 = 0f;

        for (int i = 1; i <= bulletsAmount; i++)
        {
            if (scatter)
            {
                float randomOffset = Random.Range(-angleDiff / 2, angleDiff / 2);
                angle += randomOffset;
            }

            // Calculate individual angle between the objects
            float anglePerObject = GroupAngle / (NumberPerGroup - 1);

            // Start angle
            angle2 = angle - GroupAngle * 0.5f;

            for (int j = 1; j <= NumberPerGroup; j++)
            {
                ActivateBullet(angle2);
                angle2 += anglePerObject;
            }

            angle += angleStep;
        }

        RepetitionsCnt++;
        if (RepetitionsCnt >= NumberOfRepetitions)
        {
            if (infinite)
            {
                Fire();
                CancelInvoke("RepeatedAction");
                RepetitionsTimer = RepetitionsDelay + Time.time;
            }
            else
                CancelInvoke("RepeatedAction");
        }
    }

    private void ActivateBullet(float angle)
    {
        // 5f = lifetime
        GameObject bullet = ObjectPoolManager.Instance.Get("bullet", 1.5f);

        if (bullet)
        {
            // Calculate direction vector and new position for start from radius [moveDistance].
            Vector3 direction = Quaternion.Euler(0f, 0f, (angle + transform.rotation.eulerAngles.z)) * Vector3.right;
            Vector3 newPosition = transform.position + direction * moveDistance;

            Bullet bulletComponent = bullet.GetComponent<Bullet>();

            bulletComponent.BulletColor = bulletColor;
            bulletComponent.InitialSpeed = initialSpeed;
            bulletComponent.Target = target;
            bulletComponent.MovementCurve = SpeedCurve;
            bulletComponent.RotationCurve = BulletRotationSpeedCurve;
            bulletComponent.RotationSpeed = BulletRotationSpeed;
            bulletComponent.ScaleBullet = scaleBullet;
            bulletComponent.Scale = ActivateScalerCurve;

            bulletComponent.Align = bulletAlign;
            bulletComponent.TimeToAlignment = timeToAligment;
            bulletComponent.Duration = duration;
            bulletComponent.RotationDuration = roationaDuration;
            bulletComponent.Scale = ActivateScalerCurve;

            bullet.transform.rotation = transform.rotation * Quaternion.Euler(0, 0, angle);
            bullet.transform.position = newPosition;

            bullet.SetActive(true);
        }
    }
}

