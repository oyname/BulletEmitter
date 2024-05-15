using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Revision 1.1

public class BulletEmitter : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The number of bullets to be generated in one emission.")]
    private int bulletsAmount = 10;
    [SerializeField]
    [Tooltip("The number of bullets to be generated per group.")]
    private int NumberPerGroup = 1;
    [SerializeField]
    [Tooltip("The angle at which bullets should be distributed within a group.")]
    private float GroupAngle = 0;
    [SerializeField]
    [Tooltip("The rate at which bullets should be emitted.")]
    private float repeatRate = 0.2f;
    [SerializeField]
    [Tooltip("The number of repetitions of the emission.")]
    private int NumberOfRepetitions = 1;
    private int RepetitionsCnt = 0;
    [SerializeField]
    [Tooltip("The delay between repetitions.")]
    private float RepetitionsDelay = 0f;
    private float RepetitionsTimer = 0f;
    [SerializeField]
    [Tooltip("The start-distance each bullet from center")]
    float moveDistance = 0f;
    [SerializeField]
    [Range(0f, 360f)]
    [Tooltip("The start and end angles of the emission pattern.")]
    private float startAngle = 90f, endAngle = 180f;
    [SerializeField]
    [Tooltip("Indicates whether bullets should be emitted in a scatter.")]
    private bool scatter = false;
    [SerializeField]
    [Tooltip("The target object to which bullets should be aligned.")]
    private GameObject target;
    [SerializeField]
    [Tooltip("Indicates whether bullets should be aligned towards the target object.")]
    private bool toTarget = false;
    [SerializeField]
    [Tooltip("Indicates whether oscillation of the emission should occur.")]
    private bool oscillate = false;
    [SerializeField]
    [Tooltip("The frequency of oscillation.")]
    private float frequency = 2f;
    [SerializeField]
    [Tooltip("The angle by which oscillation should occur.")]
    private float oscillateAngle = 0f;
    [SerializeField]
    [Tooltip("The opening angle of oscillation.")]
    private float oscillateOpeningAngle = 0f;
    [SerializeField]
    [Tooltip("Indicates whether the emission should be infinite.")]
    private bool infinite = true;
    [SerializeField]
    [Tooltip("The rotation speed of bullets.")]
    private float rotateSpeed = 0f;
    [SerializeField]
    [Tooltip("The initial speed of bullets.")]
    private float initialSpeed = 5f;
    [SerializeField]
    [Tooltip("The speed curve of bullets.")]
    private AnimationCurve SpeedCurve;
    [SerializeField]
    [Tooltip("The rotation speed of bullets.")]
    private float BulletRotationSpeed;
    [SerializeField]
    [Tooltip("The rotation speed curve of bullets.")]
    private AnimationCurve BulletRotationSpeedCurve;
    [SerializeField]
    [Tooltip("The scaling of bullets.")]
    private Vector3 scaleBullet = new Vector3(1f, 1f, 1f);
    [SerializeField]
    [Tooltip("Indicates whether the scaling curve should be activated.")]
    private bool ActivateScalerCurve = false;
    [SerializeField]
    [Tooltip("The scaling curve of bullets.")]
    private AnimationCurve ScalerCurve;
    [SerializeField]
    [Tooltip("Indicates whether bullets should be aligned.")]
    private bool bulletAlign = false;
    [SerializeField]
    [Tooltip("The time required for alignment.")]
    private float timeToAligment = 0f;
    [SerializeField]
    [Tooltip("The duration of the emission.")]
    private float duration = 0f;
    [SerializeField]
    [Tooltip("The duration of rotation.")]
    private float roationaDuration = 0f;
    [Tooltip("The color of bullets.")]
    public Color bulletColor;
    [Tooltip("A reference to the oscillation object.")]
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

