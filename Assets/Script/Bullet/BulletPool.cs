using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public GameObject Bullet;
    private List<Bullet> _bulletList = new List<Bullet>();

    public Bullet GetBullet() 
    {
        for (int i = 0; i < _bulletList.Count; i++)
        {
            if (!_bulletList[i].gameObject.activeInHierarchy)
            {
                return _bulletList[i];
            }
        }

        Bullet bullet = Instantiate(Bullet).GetComponent<Bullet>();
        bullet.gameObject.SetActive(false);
        _bulletList.Add(bullet);
        return bullet;
    }

    public void DestroyAll()
    {
        for (int i = 0; i < _bulletList.Count; i++)
        {
            Destroy(_bulletList[i].gameObject);
        }
    }
}
