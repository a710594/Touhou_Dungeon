using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayScale : MonoBehaviour
{
    public float GrayscaleAmount = 1;
    public SpriteRenderer SpriteRenderer;

    // Start is called before the first frame update
    public void SetScale(float scale)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        SpriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_GrayScale", scale);
        SpriteRenderer.SetPropertyBlock(mpb);
    }

    private void Start()
    {
        SetScale(GrayscaleAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
