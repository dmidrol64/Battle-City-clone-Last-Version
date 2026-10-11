using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCounterUI : MonoBehaviour
{


    [SerializeField] private Transform iconContainer;

    [SerializeField] private GameObject tankIconPrefab;

    private int currentEnemyCount = 0;


    public void InitializeIcons(int totalTanks)
    {
        currentEnemyCount = totalTanks;
        UpdateIcons();
    }


    public void OnEnemyDestroyed()
    {
        currentEnemyCount--;
        if (currentEnemyCount < 0) currentEnemyCount = 0;

        UpdateIcons();

        if (currentEnemyCount == 0)
        {
            Debug.Log("Все враги уничтожены! Уровень пройден!");
            // Здесь можно вызвать логику победы
        }
    }
    private void UpdateIcons()
    {
        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentEnemyCount; i++)
        {
            Instantiate(tankIconPrefab, iconContainer);
        }

    }
}
