using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    private float _time;
    private float _endTime = -1;
    private bool _isLoop = false;
    private Action _onTimeOutHandler;

    public Timer() { }

    public Timer(float time, Action callback, bool isLoop = false)
    {
        _time = time;
        _endTime = Time.time + time;
        _isLoop = isLoop;
        _onTimeOutHandler = callback;

        TimerUpdater.UpdateHandler += OnUpdate;
    }

    public void Start(float time, Action callback, bool isLoop = false)
    {
        _time = time;
        _endTime = Time.time + time;
        _isLoop = isLoop;
        _onTimeOutHandler = callback;

        TimerUpdater.UpdateHandler += OnUpdate;
    }

    public void Stop()
    {
        _endTime = -1;
        TimerUpdater.UpdateHandler -= OnUpdate;
    }

    public void StopLoop()
    {
        _endTime = -1;
        TimerUpdater.UpdateHandler -= OnUpdate;
    }

    private void OnUpdate()
    {
        if (_endTime != -1 && Time.time > _endTime)
        {
            if (_onTimeOutHandler != null)
            {
                _onTimeOutHandler();

                if (_isLoop)
                {
                    _endTime = Time.time + _time;
                }
                else
                {
                    _endTime = -1;
                    TimerUpdater.UpdateHandler -= OnUpdate;
                }
            }
        }
    }
}