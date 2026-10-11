using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Интерфейс UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    public List<TankDataSO> allEnemyTanks;

    private int totalScore = 0;

    private void OnEnable()
    {
        foreach (var tanks in allEnemyTanks)
        {
            tanks.OnDeathEvent += OnEnemyTankDestroyed;
        }
    }

    private void OnDisable()
    {
        foreach (var tanks in allEnemyTanks)
        {
            tanks.OnDeathEvent -= OnEnemyTankDestroyed;
        }
    }
    private void Start()
    {
        totalScore = 0;

        UpdateScoreUI();
    }

    public void OnEnemyTankDestroyed(int scoreAmount)
    {
        totalScore += scoreAmount;
        Debug.Log($"[СОБЫТИЕ] Очки получены! +{scoreAmount}. Общий счет: {totalScore}");

        if (scoreText != null)
        {
            UpdateScoreUI();
        }

    }

   public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = totalScore.ToString();
        }
    }

    public void AddScoreDirect(int scoreAmount)
    {
        totalScore += scoreAmount;
        Debug.Log($"[ОЧКИ НАПРЯМУЮ] Получено: +{scoreAmount}. Общий счет: {totalScore}");
        UpdateScoreUI();
    }
}
