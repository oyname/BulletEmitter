using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillation
{
    private float frequency;
    private float angle;
    private float openingAngle;
    private float time;

    public float Angle
    {
        get { return angle; }
        set { angle = value;}
    }

    public float Frequency
    {
        get { return frequency; }
        set { frequency = Mathf.Max(0f, value); }
    }

    public Oscillation(float frequency, float angle, float openingAngle)
    {
        this.frequency = frequency;
        this.angle = angle;
        this.openingAngle = openingAngle;
        this.time = 0f;
    }

    public void SetParameters(float frequency, float angle, float openingAngle)
    {
        this.frequency = frequency;
        this.angle = angle;
        this.openingAngle = openingAngle;
    }

    public float GetValue()
    {
        time += Time.deltaTime;
        float halfOpeningAngle = openingAngle / 2f;
        float currentAngle = Mathf.Lerp(angle - halfOpeningAngle, angle + halfOpeningAngle, (Mathf.Sin(time * frequency) + 1f) / 2f);
        return currentAngle;
    }
}
