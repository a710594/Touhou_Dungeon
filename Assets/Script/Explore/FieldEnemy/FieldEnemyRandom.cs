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
        Animator.SetBool("IsMoving", true);

        if (!ExploreController.Instance.CanMove(Vector2Int.RoundToInt(transform.position) + _currentDirection, true, false) || _currentDistance == 0)
        {
            _currentDirection = GetRandomDirection();
        }
        Vector2Int destination = Vector2Int.RoundToInt(transform.position) + _currentDirection;
        transform.DOMove((Vector2)destination, _cycleTime).SetEase(Ease.Linear);

        if (transform.position.x - destination.x > 0 && _lookAt == Vector2Int.right)
        {
            Sprite.flipX = false;
            _lookAt = Vector2Int.left;
        }
        else if (transform.position.x - destination.x < 0 && _lookAt == Vector2Int.left)
        {
            Sprite.flipX = true;
            _lookAt = Vector2Int.right;
        }

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
