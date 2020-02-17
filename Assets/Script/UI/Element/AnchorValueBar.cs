using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorValueBar : ValueBar
{
    private Transform _anchor;

    public void SetAnchor(Transform anchor)
    {
        _anchor = anchor;
    }

    void Update()
    {
        if (_anchor != null)
        {
            this.transform.position = Camera.main.WorldToScreenPoint(_anchor.position);
        }
    }
}
