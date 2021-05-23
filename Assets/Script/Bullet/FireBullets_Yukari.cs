using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullets_Yukari : FireBullets
{
    private int _count = 0;
    private float _time = 2f;
    private float _startTime;

    public override void StartFire()
    {
        InvokeRepeating("Fire", 0f, 0.01f);
        _startTime = Time.time;
    }

    protected override void Fire()
    {
        float angleStep = 360 / Amount;
        float angle = angleStep*_count;
        _count = (_count + 1) % Amount;

        float directionX;
        float directionY;
        Vector2 direction;

        if (_count % 10 == 0)
        {
            angle += angleStep;
            return;
        }

        directionX = Mathf.Cos((angle * Mathf.PI) / 180f);
        directionY = Mathf.Sin((angle * Mathf.PI) / 180f);
        direction = new Vector2(directionX, directionY);
        direction = direction.normalized;

        Bullet bullet = BulletPool.GetBullet();
        bullet.transform.position = transform.position + (Vector3)direction * 2f;
        bullet.transform.eulerAngles = new Vector3(0, 0, angle + 90);
        bullet.gameObject.SetActive(true);
        bullet.SetData(direction, 2, 3f, 3f, 0, 0);
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
