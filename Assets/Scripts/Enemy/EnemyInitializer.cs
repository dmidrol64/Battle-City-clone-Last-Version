using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Analytics.IAnalytic;

public class EnemyInitializer : MonoBehaviour
{
    

    public void SetupTank(TankDataSO tankData, bool isBonus)
    {
        if (tankData == null) return;

        LivesComponent lives = GetComponent<LivesComponent>();
        if(lives != null)
        {
            lives.InitHealth(tankData.maxHits,tankData);
        }
        Shoot weapon = GetComponentInChildren<Shoot>();
        if (weapon != null)
        {
            weapon.levelWeapon = tankData.weaponLevel;
            weapon.rate = tankData.fireRate;
        }

        // Ищем новый выделенный компонент эффекта
        BonusTankEffect bonusEffect = GetComponent<BonusTankEffect>();
        if (bonusEffect != null)
        {
            // Передаем статус бонуса (мигать или не мигать) 
            bonusEffect.InitBonusStatus(isBonus);
        }


        EnemyMove move = GetComponent<EnemyMove>();
        if (move != null)
        {
            move.InitTank(tankData); 
            
        }
        else
        {
            Debug.LogWarning("EnemyMove не найден на этом объекте!");
        }

        Transform foundBulletPlace = transform.Find("VisualPlace");

        if (foundBulletPlace == null)
        {
            // Если не нашли в корне, ищем во всех дочерних объектах (внутри графики префаба)
            foundBulletPlace = transform.GetComponentInChildren<Transform>().Find("VisualPlace");
        }

        if (foundBulletPlace != null && weapon != null)
        {
            weapon.bulletPlace = foundBulletPlace;
        }
  
    }
}
