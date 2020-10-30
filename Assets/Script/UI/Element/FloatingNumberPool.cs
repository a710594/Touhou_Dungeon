using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNumberPool : MonoBehaviour
{
    private class FloatingNumberData
    {
        public string Text;
        public FloatingNumber.Type Type;

        public FloatingNumberData(string text, FloatingNumber.Type type)
        {
            Text = text;
            Type = type;
        }
    }

    public FloatingNumber FloatingNumber;
    public float CycleTime; //每幾秒生一個
    public float Height;
    public float Duration; //顯示幾秒

    private bool _isLock = false;

    private Queue<FloatingNumber> _poolQueue = new Queue<FloatingNumber>();
    private Queue<FloatingNumberData> _dataQueue = new Queue<FloatingNumberData>();

    public void SetAnchor(Transform anchor)
    {
        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
    }

    public void Play(string text, FloatingNumber.Type type/*, Action beforeCallback, Action afterCallBack*/) //beforeCallback:跳數字的同時會發生的事件 afterCallBack:數字消失時會發生的事件
    {
        if (!_isLock)
        {
            _isLock = true;
            Debug.Log("dequeue");
            FloatingNumber floatingNumber = _poolQueue.Dequeue();
            floatingNumber.Play(text, type, transform.position);

            //if (beforeCallback != null)
            //{
            //    beforeCallback();
            //}

            Timer unlockTimer = new Timer(CycleTime, () => //顯示下一個數字
            {
                _isLock = false;
                if (_dataQueue.Count > 0)
                {
                    FloatingNumberData data = _dataQueue.Dequeue();
                    Play(data.Text, data.Type/*, data.BeforeCallback, data.AfterCallback*/);
                }
            });

            Timer recycleTimer = new Timer(Duration, () => //當前的數字消失
            {
                Debug.Log("enqueue");
                _poolQueue.Enqueue(floatingNumber);

                //if (afterCallBack != null)
                //{
                //    afterCallBack();
                //}
            });
        }
        else
        {
            FloatingNumberData data = new FloatingNumberData(text, type);
            _dataQueue.Enqueue(data);
        }
    }

    void Awake()
    {
        FloatingNumber.SetValue(Height, Duration);
        _poolQueue.Enqueue(FloatingNumber);

        int amount = (int)Math.Ceiling(Duration / CycleTime);
        FloatingNumber clone;
        for (int i=0; i<amount; i++)
        {
            clone = ResourceManager.Instance.Spawn(FloatingNumber.gameObject).GetComponent<FloatingNumber>();
            clone.transform.SetParent(this.transform);
            clone.transform.localPosition = Vector2.zero;
            clone.SetValue(Height, Duration);
            _poolQueue.Enqueue(clone);
        }
    }

    void Update()
    {
    }
}
