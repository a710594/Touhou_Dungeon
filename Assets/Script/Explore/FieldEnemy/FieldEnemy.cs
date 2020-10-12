using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FieldEnemy : MonoBehaviour
{
    public SpriteRenderer Sprite;
    public Animator Animator;
    public Action<BattleGroupData.RootObject> OnPlayerEnterHandler;

    protected Vector2Int _lookAt = Vector2Int.left;
    protected float _cycleTime = 0.5f; //移動的週期
    protected BattleGroupData.RootObject _data;
    protected Timer _timer = new Timer();

    public virtual void Move() { }

    public void Stop()
    {
        _timer.Stop();
        transform.DOKill();
    }

    public void Continue()
    {
        _timer.Start(_cycleTime, Move, true);
    }

    public virtual void Init(int battleGroupId, Vector2 position)
    {
        _data = BattleGroupData.GetData(battleGroupId);
        Animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animator/" + _data.Animator);
        _cycleTime = 0.5f;
        transform.position = position;
        _timer.Start(_cycleTime, Move, true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (OnPlayerEnterHandler != null)
            {
                OnPlayerEnterHandler(_data);
            }
        }
    }

    private void OnDestroy()
    {
        _timer.Stop();
    }

    private void Awake()
    {
        //_timer.Start(_cycleTime, Move, true);
    }
}
