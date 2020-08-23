using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorValueBar : ValueBar
{
    public GameObject[] HPQueue;

    private Transform _anchor;

    public void SetAnchor(Transform anchor)
    {
        _anchor = anchor;
    }

    public void SetHPQueue(int count)
    {
        for (int i=0; i<HPQueue.Length; i++)
        {
            HPQueue[i].SetActive(i < count);
        }
    }

    protected override void UpdateData()
    {
        base.UpdateData();
        if (_anchor != null)
        {
            this.transform.position = Camera.main.WorldToScreenPoint(_anchor.position);
        }
    }
}
