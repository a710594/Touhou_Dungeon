using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingNumberData
{
    public FloatingNumber.Type Type;
    public string Text;

    public FloatingNumberData(FloatingNumber.Type type, string text)
    {
        Type = type;
        Text = text;
    }
}
