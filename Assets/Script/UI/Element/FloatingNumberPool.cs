using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNumberPool : MonoBehaviour
{
    private class Data
    {
        public string Text;
        public FloatingNumber.Type Type;

        public Data(string text, FloatingNumber.Type type)
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
    private Queue<Data> _dataQueue = new Queue<Data>();

    public void SetAnchor(Transform anchor)
    {
        transform.SetParent(anchor);
        transform.localPosition = Vector3.zero;
    }

    public void Play(string text, FloatingNumber.Type type)
    {
        if (!_isLock)
        {
            _isLock = true;
            Debug.Log("dequeue");
            FloatingNumber floatingNumber = _poolQueue.Dequeue();
            floatingNumber.Play(text, type, transform.position);

            Timer unlockTimer = new Timer(CycleTime, () => //顯示下一個數字
            {
                _isLock = false;
                if (_dataQueue.Count > 0)
                {
                    Data data = _dataQueue.Dequeue();
                    Play(data.Text, data.Type);
                }
            });

            Timer recycleTimer = new Timer(Duration, () => //當前的數字消失
            {
                Debug.Log("enqueue");
                _poolQueue.Enqueue(floatingNumber);
            });
        }
        else
        {
            Data data = new Data(text, type);
            _dataQueue.Enqueue(data);
        }
    }

    public void Play(List<FloatingNumberData> list)
    {
        Data data;
        for (int i=0; i< list.Count; i++)
        {
            data = new Data(list[i].Text, list[i].Type);
            _dataQueue.Enqueue(data);
        }


        if (!_isLock)
        {
            Play(_dataQueue.Dequeue());
        }
    }

    private void Play(Data data)
    {
        _isLock = true;
        FloatingNumber floatingNumber = _poolQueue.Dequeue();
        floatingNumber.Play(data.Text, data.Type, transform.position);

        Timer unlockTimer = new Timer(CycleTime, () => //顯示下一個數字
        {
            _isLock = false;
            if (_dataQueue.Count > 0)
            {
                Play(_dataQueue.Dequeue());
            }
        });

        Timer recycleTimer = new Timer(Duration, () => //當前的數字消失
        {
            Debug.Log("enqueue");
            _poolQueue.Enqueue(floatingNumber);
        });
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
