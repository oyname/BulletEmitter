using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Messenger : MonoBehaviour
{
    private static Messenger instance;

    private AnimationCurve scaleCurve = null;
    private float startTime = 0f;
    private float scaleValue = 0f;

    public float StartTime
    {
        set { startTime = value; }
    }

    public float ScaleValue
    {
        get { return scaleValue; }
    }

    public AnimationCurve ScaleCurve
    {
        set { scaleCurve = value; }
    }

    public static Messenger Instance
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<Messenger>();

                if(instance==null)
                {
                    GameObject singeltonObject = new GameObject();
                    instance = singeltonObject.AddComponent<Messenger>();
                    DontDestroyOnLoad(singeltonObject);
                }
            }

            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float timeSinceStart = Time.time - startTime;
        if (scaleCurve != null)
        {
            scaleValue = scaleCurve.Evaluate(timeSinceStart);
        }
    }
}
