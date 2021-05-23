using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Rigidbody2D Rigidbody2D;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("?");
    }

    private void Start()
    {

    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Rigidbody2D.AddForce(transform.right * 5);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            Rigidbody2D.AddForce(transform.up * 10);
        }

    }

}
