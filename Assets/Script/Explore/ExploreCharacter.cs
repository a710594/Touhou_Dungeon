﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExploreCharacter : MonoBehaviour
{
    public SpriteRenderer Sprite;
    public Animator Animator;

    private bool _isMoving = false;
    private Vector2Int _lookAt = Vector2Int.left;

    public void Move(Vector2Int direction, Action callback = null)
    {
        if (_isMoving /*|| CheckCollider(direction)*/)
        {
           return;
        }

        Vector2Int position = Vector2Int.RoundToInt(transform.position) + direction;

        _isMoving = true;
        Animator.SetBool("IsMoving", true);
        if (transform.position.x - position.x > 0 && _lookAt == Vector2Int.right)
        {
            Sprite.flipX = false;
            _lookAt = Vector2Int.left;
        }
        else if (transform.position.x - position.x < 0 && _lookAt == Vector2Int.left)
        {
            Sprite.flipX = true;
            _lookAt = Vector2Int.right;
        }

        transform.DOMove((Vector2)position, 0.2f).SetEase(Ease.Linear).OnComplete(() =>
        {
            _isMoving = false;
            if (callback != null)
            {
                callback();
            }
        });
    }

    public void Pause()
    {
        Animator.SetBool("IsMoving", false);
    }

    public void Stop()
    {
        transform.DOKill();
    }

    private bool CheckCollider(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(Sprite.transform.position, direction, 1);
        if (hit.collider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            Move(Vector2Int.right);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            Move(Vector2Int.up);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            Move(Vector2Int.down);
        }
    }
}