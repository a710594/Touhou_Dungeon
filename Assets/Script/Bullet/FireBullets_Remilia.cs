using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullets_Remilia : FireBullets
{
    private int _count = 0;
    private float _time = 2f;
    private float _startTime;

    public override void StartFire()
    {
        InvokeRepeating("Fire", 0f, 0.2f);
        _startTime = Time.time;
    }

    protected override void Fire()
    {
        float angleStep = 360 / Amount;
        float angle = 0;

        float directionX;
        float directionY;
        Vector2 direction;
        for (int i = 0; i < Amount; i++)
        {
            directionX = Mathf.Cos((angle * Mathf.PI) / 180f);
            directionY = Mathf.Sin((angle * Mathf.PI) / 180f);
            direction = new Vector2(directionX, directionY);
            direction = direction.normalized;

            Debug.Log(i);
            Bullet bullet = BulletPool.GetBullet();
            bullet.transform.position = transform.position + (Vector3)direction * 2f;
            bullet.transform.eulerAngles = new Vector3(0, 0, angle - 90);
            bullet.gameObject.SetActive(true);
            bullet.SetData(direction, 3, 5f, 5f, 0, 1);
            angle += angleStep;
        }

        _count++;
        if (_count == 5)
        {
            CancelInvoke("Fire");
        }
    }

    private void Start()
    {
        StartFire();
    }

    private void Update()
    {
        if (Time.time - _startTime > _time)
        {
            BulletPool.DestroyAll();
            Destroy(gameObject);
        }
    }
}
