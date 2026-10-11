using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteelWall : MonoBehaviour,IDamageable
{

    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        if(damagePower >= 3)
        {
            Destroy(gameObject);
        }
    }
}
