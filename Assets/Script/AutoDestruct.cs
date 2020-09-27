using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestruct : MonoBehaviour
{
    public float Duration;

    private float _startTime;

    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - _startTime > Duration)
        {
            Destroy(gameObject);
        }
    }
}
