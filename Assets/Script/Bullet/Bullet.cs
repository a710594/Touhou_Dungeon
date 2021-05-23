using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 _direction;
    private float _destroyTime;
    private float _startSpeed;
    private float _finalSpeed;
    private float _accelerateTime;
    private float _startTime;
    private float _offset;

    public void SetData(Vector2 direction, float time, float startSpeed, float finalSpeed, float accelerateTime, float offset)
    {
        _direction = direction;
        _destroyTime = time;
        _startSpeed = startSpeed;
        _finalSpeed = finalSpeed;
        _accelerateTime = accelerateTime;
        _offset = offset;
        _startTime = Time.time;

        if (_accelerateTime == 0)
        {
            _accelerateTime = 1;
        }

        Invoke("Destroy", _destroyTime);
    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        _direction += Vector2.Perpendicular(_direction) * _offset * Time.deltaTime;
        _direction = _direction.normalized;
        transform.position += (Vector3)_direction * (Mathf.Lerp(_startSpeed, _finalSpeed, (Time.time - _startTime) / _accelerateTime)) * Time.deltaTime;
    }

    private void Destroy()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }
}
