using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    // Просто объявляем имя метода и параметры. В конце СТРОГО точка с запятой!

    void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition);




}
