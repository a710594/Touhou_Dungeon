using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public AnchorValueBar AnchorValueBar;

    private void Start()
    {
        AnchorValueBar.SetValue(50, 100);
        AnchorValueBar.SetPrediction(50, 30, 100);
    }
}
