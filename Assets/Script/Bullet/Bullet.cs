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

    public void SetData(Vector2 direction, float time, float startSpeed, float finalSpeed, float accelerateTime)
    {
        _direction = direction;
        _destroyTime = time;
        _startSpeed = startSpeed;
        _finalSpeed = finalSpeed;
        _accelerateTime = accelerateTime;
        _startTime = Time.time;

        Invoke("Destroy", _destroyTime);
    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        //transform.Translate(_direction * _speed * Time.deltaTime);
        //float step = _speed * Time.deltaTime; // calculate distance to move
        //transform.position = Vector3.MoveTowards(transform.position, transform.position + (Vector3)_direction, step);
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
