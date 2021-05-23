using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullets_Sample : FireBullets
{
    public float StartAngle = 90;
    public float EndAngle = 270;

    public override void StartFire()
    {
        InvokeRepeating("Fire", 0f, 2f);
    }

    protected override void Fire() 
    {
        float angleStep = (EndAngle - StartAngle) / Amount;
        float angle = StartAngle;

        float directionX;
        float directionY;
        Vector2 direction;
        for (int i=0; i<Amount + 1; i++) 
        {
            directionX = Mathf.Cos((angle * Mathf.PI) / 180f);
            directionY = Mathf.Sin((angle * Mathf.PI) / 180f);
            direction = new Vector2(directionX, directionY);
            direction = direction.normalized;

            Bullet bullet = BulletPool.GetBullet();
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.gameObject.SetActive(true);
            bullet.SetData(direction, 3, 1, 1, 0, 0);
            angle += angleStep;
        }
    }
}
