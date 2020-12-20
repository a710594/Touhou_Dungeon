using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelay : MonoBehaviour
{
    public AudioSource AudioSource;
    public float DelayTime;

    private bool _isPlayed = false;
    private float _startTime;

    // Start is called before the first frame update
    void Start()
    {
        _startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isPlayed && Time.time - _startTime > DelayTime)
        {
            AudioSource.Play();
            _isPlayed = true;
        }
    }
}
