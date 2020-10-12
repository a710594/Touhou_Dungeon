using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance = null;

    public float Speed = 10;
    public ViewDrag ViewDrag;

    private float _originalZ;
    private Vector3 _currentPosition = new Vector3();

    public void SetParent(Transform parent, bool isTween, Action callback = null)
    {
        if (parent != transform.parent)
        {
            transform.parent = parent;
            _currentPosition.x = parent.position.x;
            _currentPosition.y = parent.position.y;
            _currentPosition.z = _originalZ;

            if (isTween)
            {
                transform.DOMove(_currentPosition, Vector2.Distance(transform.position, parent.position) / Speed).OnComplete(() =>
                {
                    if (callback != null)
                    {
                        callback();
                    }
                });
            }
            else
            {
                transform.position = _currentPosition;
                if (callback != null)
                {
                    callback();
                }
            }
        }
        else
        {
            if (callback != null)
            {
                callback();
            }
        }
    }

    public void StartDrag(Vector3 mousePosition)
    {
        ViewDrag.StartDrag(mousePosition);
    }

    public void OnDrag(Vector3 mousePosition)
    {
        ViewDrag.OnDrag(mousePosition);
    }

    public void EndDrag()
    {
        ViewDrag.EndDrag();
    }

    public void CameraMove(Vector2Int direction)
    {
        int layerMask = 1 << LayerMask.NameToLayer("Camera");
        layerMask = ~layerMask;
        Debug.Log(LayerMask.NameToLayer("Camera"));
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.transform.position, (Vector2)direction, 0.6f, layerMask);
        if (hit.collider == null)
        {
            Camera.main.transform.position += new Vector3(direction.x, direction.y, 0) * Time.deltaTime * 5f;
        }
    }

    void Awake()
    {
        _originalZ = transform.position.z;
        Instance = this;
    }
}
