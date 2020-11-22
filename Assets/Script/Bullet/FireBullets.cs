using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBullets : MonoBehaviour
{
    public BulletPool BulletPool;
    public int Amount = 10;

    public virtual void StartFire()
    {
    }

    protected virtual void Fire()
    {
    }
}
