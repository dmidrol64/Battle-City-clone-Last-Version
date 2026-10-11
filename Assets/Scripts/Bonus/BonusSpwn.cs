using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusSpwn : MonoBehaviour
{

    [Header("Главный шаблон")]
    [SerializeField] private GameObject baseBonusContainerPrefab;

    [Header("База данных бонусов")]
    // Вместо списка префабов теперь храним список файлов настроек ScriptableObject!
    [SerializeField] private List<BonusDataSO> allBonusesData = new List<BonusDataSO>();

    public void BonusInst()
    {

        if (baseBonusContainerPrefab == null || allBonusesData == null || allBonusesData.Count == 0)
        {
            return;
        }
       
            Vector2 randomPoint = new Vector2(Random.Range(-28f, 22.27f), Random.Range(-9.4f,11.6f));

            int randomPrefab = Random.Range(0, allBonusesData.Count);
        
            BonusDataSO selectedBonusData = allBonusesData[randomPrefab];
        
        GameObject bonusContainer = Instantiate(baseBonusContainerPrefab, randomPoint, Quaternion.identity);

        PickupBonus pickupScript = bonusContainer.GetComponent<PickupBonus>();
        if (pickupScript != null)
        {
            pickupScript.InitBonus(selectedBonusData); // Метод инициализации 
        }

    }

}
