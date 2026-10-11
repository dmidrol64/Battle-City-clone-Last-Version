using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum BonusType { Star, Helmet, Timer, Pistol, Tank, Granade, Shovel }
public class PickupBonus : MonoBehaviour
{

    [SerializeField] private BonusDataSO data;


    public void InitBonus(BonusDataSO bonusData)
    {
        data = bonusData;
        if (data == null) return;

        Destroy(gameObject, data.lifetime);

        
          GameObject visual = Instantiate(data.visualPrefab, transform.position, Quaternion.identity, transform);
          visual.transform.localPosition = Vector3.zero;//спавним внутри родителя 
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
         TankBonusHandler handler = collision.GetComponent<TankBonusHandler>();
    
    if (handler != null && data != null)
    {
        // Передаем тип бонуса напрямую из ScriptableObject
        handler.ActivateBonus(data.bonusType);

        // Уничтожаем бонус со сцены
        Destroy(gameObject);
    }
    else if (data == null)
    {
       
        Debug.LogWarning($"[ВНИМАНИЕ] Объект {gameObject.name} подобран, но его ScriptableObject (data) равен null!");
        Destroy(gameObject); //удаляем пустой контейнер, чтобы не засорял сцену
    }
    }
}
