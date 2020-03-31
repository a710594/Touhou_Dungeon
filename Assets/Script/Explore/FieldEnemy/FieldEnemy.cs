using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldEnemy : MonoBehaviour
{
    public Action OnPlayerEnterHandler;

    protected float _cycleTime = 0.5f; //移動的週期
    protected Vector2Int _currentDirection;
    protected List<Vector2Int> _directionList = new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };
    protected Timer _timer = new Timer();

    public virtual void Move() { }

    public void Stop()
    {
        _timer.Stop();
        transform.DOKill();
    }

    protected virtual void Init() { }

    protected Vector2Int GetRandomDirection()
    {
        Vector2Int direction;
        List<Vector2Int> _tempList = new List<Vector2Int>();

        _tempList = new List<Vector2Int>(_directionList);
        while (_tempList.Count > 0)
        {
            direction = _tempList[UnityEngine.Random.Range(0, _tempList.Count)];
            if (ExploreController.Instance.CanMove(Vector2Int.RoundToInt(transform.position) + direction, true, false))
            {
                return direction;
            }
            else
            {
                _tempList.Remove(direction);
            }
        }
        return Vector2Int.zero;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (OnPlayerEnterHandler != null)
            {
                OnPlayerEnterHandler();
            }
        }
    }

    private void OnDestroy()
    {
        _timer.Stop();
    }

    private void Awake()
    {
        Init();
        _timer.Start(_cycleTime, Move, true);
    }
}
