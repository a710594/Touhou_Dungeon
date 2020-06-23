using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldEnemyRandom : FieldEnemy
{
    private enum State
    {
        Patrol,
        Chase,
    }

    public int MaxChangeDistance = 5; //換移動方向的最大距離
    public int VisionDistance = 5; //發現玩家的距離

    private State _currentState;
    private int _currentDistance = 0; //移動一段距離後才會改變方向
    private int _chaseStep =10;
    private Vector2Int _currentDirection;
    private Transform _player;
    private List<Vector2Int> _mapList = new List<Vector2Int>();
    private List<Vector2Int> _directionList = new List<Vector2Int>() { Vector2Int.left, Vector2Int.right, Vector2Int.up, Vector2Int.down };

    public override void Init(int battleGroupId, Vector2 position)
    {
        base.Init(battleGroupId, position);
        _currentDirection = GetRandomDirection();
        _currentDistance = Random.Range(1, MaxChangeDistance + 1);
    }

    public override void Move()
    {
        Vector2Int destination;

        Animator.SetBool("IsMoving", true);

        if (_currentState == State.Patrol && Utility.GetDistance(transform.position, _player.transform.position) <= 5)
        {
            _chaseStep = 10;
            _currentState = State.Chase;
            Debug.Log("Chase");
        }
        else if (_currentState == State.Chase && _chaseStep == 0)
        {
            _currentState = State.Patrol;
            Debug.Log("Patrol");
        }

        if (_currentState == State.Patrol)
        {
            if (!ExploreController.Instance.CanMove(Vector2Int.RoundToInt(transform.position) + _currentDirection, true, false) || _currentDistance == 0)
            {
                _currentDirection = GetRandomDirection();
            }
            destination = Vector2Int.RoundToInt(transform.position) + _currentDirection;
        }
        else
        {
            destination = GetChasePosition();
        }

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
            _currentDistance = Random.Range(1, MaxChangeDistance + 1);
        }
        else
        {
            _currentDistance--;
        }
    }

    public void SetData(Transform player, List<Vector2Int> mapList)
    {
        _player = player;
        _mapList = mapList;
    }

    private Vector2Int GetRandomDirection() //回傳的是"方向"
    {
        Vector2Int myPosition = Vector2Int.RoundToInt(transform.position);
        Vector2Int direction;
        List<Vector2Int> tempList = new List<Vector2Int>(_directionList);

        while (tempList.Count > 0)
        {
            direction = tempList[UnityEngine.Random.Range(0, tempList.Count)];
            if (ExploreController.Instance.CanMove(Vector2Int.RoundToInt(myPosition) + direction, true, false))
            {
                return direction;
            }
            else
            {
                tempList.Remove(myPosition + direction);
            }
        }
        return myPosition;
    }


    private Vector2Int GetChasePosition() //回傳的是"位置"
    {
        Vector2Int myPosition = Vector2Int.RoundToInt(transform.position);
        Vector2Int playerPosition = Vector2Int.RoundToInt(_player.transform.position);
        //Vector2Int direction;
        //List<Vector2Int> tempList = new List<Vector2Int>(_directionList);
        //for (int i = 0; i < tempList.Count; i++)
        //{
        //    direction = tempList[i];
        //    if (!ExploreController.Instance.CanMove(myPosition + direction, true, false))
        //    {
        //        tempList.Remove(direction);
        //    }
        //}

        //direction = tempList[0];
        //for (int i = 1; i < tempList.Count; i++)
        //{
        //    if (Utility.GetDistance(playerPosition, myPosition + direction) > Utility.GetDistance(playerPosition, myPosition + tempList[i]))
        //    {
        //        direction = tempList[i];
        //    }
        //}

        //_chaseStep--;
        //Debug.Log(direction);

        _chaseStep--;
        List<Vector2Int> pathList = AStarAlgor.Instance.GetPath(myPosition, playerPosition, _mapList, true);

        if (pathList != null)
        {
            return pathList[1];
        }
        else
        {
            return myPosition;
        }
    }
}
