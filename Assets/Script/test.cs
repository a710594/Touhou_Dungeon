using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    private Timer _timer = new Timer();

    private void Start()
    {
        _timer.Start(1, ()=> 
        {
            Debug.Log("1");
            _timer.Start(1, ()=> 
            {
                Debug.Log("2");
            });
        });
    }
}
