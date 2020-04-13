using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldEnemyRandom : FieldEnemy
{
    public int MaxDistance = 5;

    //隨機移動
    private int _currentDistance = 0; //移動一段距離後才會改變方向

    public override void Move()
    {
        if (!ExploreController.Instance.CanMove(Vector2Int.RoundToInt(transform.position) + _currentDirection, true, false) || _currentDistance == 0)
        {
            _currentDirection = GetRandomDirection();
        }
        Vector2Int position = Vector2Int.RoundToInt(transform.position) + _currentDirection;
        transform.DOMove((Vector2)position, _cycleTime).SetEase(Ease.Linear);

        if (_currentDistance == 0)
        {
            _currentDistance = Random.Range(1, MaxDistance + 1);
        }
        else
        {
            _currentDistance--;
        }
    }

    public override void Init(int battleGroupId, Vector2 position)
    {
        base.Init(battleGroupId, position);
        _currentDirection = GetRandomDirection();
        _currentDistance = Random.Range(1, MaxDistance + 1);
    }
}
