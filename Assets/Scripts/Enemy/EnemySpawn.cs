using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class EnemySpawn : MonoBehaviour
{
    [Header("Эффекты")]
    [SerializeField] public GameObject spawnEffectPrefab; // Сюда закинь префаб мигающей звезды

    [Header("Списки пула и волны")]
    public List<TankDataSO> enemyPool = new List<TankDataSO>();

    [System.NonSerialized]
    public List<GameObject> activeTank = new List<GameObject>();
    Queue<TankDataSO> tankQueue = new Queue<TankDataSO>();

    Tilemap[] _allTileMap;
    private int totalSpawnedTank = 0;

    public static bool IsTimeFrozen = false;

    int spawnAttempts = 10;
    private void Awake()
    {
        _allTileMap = FindObjectsByType<Tilemap>(FindObjectsInactive.Exclude);
    }
    void Start()
    {
        
        if (enemyPool == null || enemyPool.Count == 0)
        {
            Debug.LogError("[ОШИБКА СПАВНЕРА] Список enemyPool пуст! Заполни его файлами TankDataSO в Инспекторе.");
            return;
        }

        // Создаем под каждый тип по 4 танка заранее в пуле
        foreach (var tankData in enemyPool)
        {
            if (tankData != null)
            {
                // Наполняем пул, делая этот спавнер родителем для спящих танков в иерархии
                tankData.PrewarmPool(transform, 4);
            }
        }

        // очередь из 16 танков на основе весов
        for (int i = 0; i < 16; i++)
        {
            TankDataSO randomTankData = GetRandomTankByWeight();
            if (randomTankData != null)
            {
                tankQueue.Enqueue(randomTankData);
            }
        }

        EnemyCounterUI counterUI = FindAnyObjectByType<EnemyCounterUI>();
        if (counterUI != null)
        {
            counterUI.InitializeIcons(16);
        }

       

        StartCoroutine(CreateTank());
    }

    IEnumerator CreateTank()
    {
        while (true)
        {
            // Каждую итерацию принудительно очищаем список от уничтоженных 
            // или выключенных в пул танков (защита от "призраков")
            activeTank.RemoveAll(tank => tank == null || !tank.activeInHierarchy);
            if (activeTank.Count < 4 && tankQueue.Count > 0)
            {
                float randomX = UnityEngine.Random.Range(-29f, 21f);
                Vector3 rand = new Vector3(randomX, 12f, 0f);

                
                TankDataSO tankPrefab = tankQueue.Dequeue();
                totalSpawnedTank++;

                // номера бонусных танков
                bool isBonus = (totalSpawnedTank == 4 || totalSpawnedTank == 9 || totalSpawnedTank == 13);

                // Запускаем кнопку спавна
                SpawnEnemyTank(rand, tankPrefab, isBonus);

                yield return new WaitForSeconds(2f);
            }
            else
            {
                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    public void SpawnEnemyTank(Vector3 spawnPosition, TankDataSO tankData, bool isBonus)
    {
        if (tankData == null) return;
        StartCoroutine(SpawnSequenceRoutine(spawnPosition, tankData, isBonus));
    }

    private IEnumerator SpawnSequenceRoutine(Vector3 position, TankDataSO tankData, bool isBonus)
    {
      
            // Спавним мигающую звезду в указанной точке карты
            GameObject effectClone = Instantiate(spawnEffectPrefab, position, Quaternion.identity);

            //Ждем ровно 0.5 секунды, пока звезда мигает на экране
            yield return new WaitForSeconds(0.5f);

            // Время вышло! Удаляем звезду со сцены
            if (effectClone != null)
            {
                Destroy(effectClone);
            }

            //материализуем танк врага из пула строго на месте исчезнувшей звезды (разворот 180° — вниз)
            Quaternion downRotation = Quaternion.Euler(0f, 0f, 180f);
            GameObject newEnemy = tankData.GetTank(position, downRotation, isBonus);

            // Добавляем танк в список только если пул успешно вернул объект
            if (newEnemy != null)
            {
                activeTank.Add(newEnemy);
            }
            else
            {
                Debug.LogError($"[ОШИБКА ПУЛА] Не удалось достать свободный танк типа {tankData.name} из пула! Возможно, размер пула слишком мал.");
            }
        
       
    }

    private TankDataSO GetRandomTankByWeight()
    {
        if (enemyPool == null || enemyPool.Count == 0) return null;

        int totalWeight = 0;
        //складываем все веса
        foreach (var tank in enemyPool)
        {
            totalWeight += tank.spawnWeight;
        }

        int randomValue = UnityEngine.Random.Range(0, totalWeight);
        int currentWeightSum = 0;

        foreach (var tank in enemyPool)
        {
            currentWeightSum += tank.spawnWeight;
            //сравниваем рандмный вес с весами танков
            if (randomValue < currentWeightSum)
            {
                return tank;
            }
        }
        return enemyPool[0];
    }

    public void RemoveDeadTank(GameObject deadTank)
    {
        if (activeTank.Contains(deadTank))
        {
            activeTank.Remove(deadTank);
        }
    }
}
