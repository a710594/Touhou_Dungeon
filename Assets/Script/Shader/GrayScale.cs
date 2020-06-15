using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrayScale : MonoBehaviour
{
    public float GrayscaleAmount = 1;
    public SpriteRenderer SpriteRenderer;
    public Material Material;

    // Start is called before the first frame update
    public void SetScale(float scale)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        SpriteRenderer.material = Material;
        SpriteRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_GrayScale", scale);
        SpriteRenderer.SetPropertyBlock(mpb);
    }

    private void Start()
    {
        //(GrayscaleAmount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
