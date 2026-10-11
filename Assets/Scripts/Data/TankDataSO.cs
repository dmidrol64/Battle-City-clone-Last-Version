using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTankData", menuName = "Tanks/TankData")]
public class TankDataSO : ScriptableObject
{
    [Header("Базовые префабы")]
    public GameObject baseContainerPrefab; // Корневой пустой контейнер (с Rigidbody2D, EnemyInitializer и т.д.)
    public GameObject visualTankPrefab;          // Графическая модель танка (визуальная часть)

    [Header("Характеристики баланса")]
    public int maxHits = 1;
    public int weaponLevel = 0;
    public float fireRate = 1.5f;
    public int spawnWeight = 50; 
    public float moveSpeed = 3f;
    public int scoreReward = 100;

    public System.Action<int> OnDeathEvent;

    [System.NonSerialized]
    private List<GameObject> sleepTanks = new List<GameObject>();

   
    public void PrewarmPool(Transform poolContainer, int count)
    {
        // Перед запуском чистим старые мета-ссылки редактора Unity
        sleepTanks.Clear();

        OnDeathEvent = null;

        for (int i = 0; i < count; i++)
        {
            // Спавним корневой контейнер строго как дочерний элемент нашего Спавнера
            GameObject tankContainer = Instantiate(baseContainerPrefab, poolContainer);

            // Если у танка есть графика, спавним её ОДИН РАЗ сразу внутрь контейнера
            if (visualTankPrefab != null)
            {
                Instantiate(visualTankPrefab, tankContainer.transform.position, tankContainer.transform.rotation, tankContainer.transform);
            }

            // Усыпляем танк и убираем его в пул
            tankContainer.SetActive(false);
            sleepTanks.Add(tankContainer);
        }
    }

    // 2. ВЫДАЧА ТАНКА ИЗ ПУЛА
    public GameObject GetTank(Vector3 position, Quaternion rotation, bool isBonus)
    {
        for (int i = 0; i < sleepTanks.Count; i++)
        {
            // Находим свободный спящий танк
            if (sleepTanks[i] != null && !sleepTanks[i].activeInHierarchy)
            {
                GameObject readyTank = sleepTanks[i];

                // Отвязываем танк от родительского спавнера, чтобы он двигался по карте свободно
                readyTank.transform.SetParent(null);

                // Перемещаем на место вспыхнувшей звезды
                readyTank.transform.position = position;
                readyTank.transform.rotation = rotation;

                // Пробуждаем танк
                readyTank.SetActive(true);

                // Запускаем инициализатор параметров ИИ (жизни, скорость, пушка)
                EnemyInitializer initializer = readyTank.GetComponent<EnemyInitializer>();
                if (initializer != null)
                {
                    initializer.SetupTank(this, isBonus);
                }

                return readyTank;
            }
        }

        // Если волна идет слишком быстро и все танки данного типа заняты, 
        // динамически расширяем пул на 1 танк в целях безопасности игры
        GameObject dynamicTank = Instantiate(baseContainerPrefab, position, rotation);
        if (visualTankPrefab != null)
        {
            Instantiate(visualTankPrefab, dynamicTank.transform.position, dynamicTank.transform.rotation, dynamicTank.transform);
        }

        EnemyInitializer dynamicInit = dynamicTank.GetComponent<EnemyInitializer>();
        if (dynamicInit != null)
        {
            dynamicInit.SetupTank(this, isBonus);
        }

        sleepTanks.Add(dynamicTank);
        return dynamicTank;
    }
}
