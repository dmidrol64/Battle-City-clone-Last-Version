using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesComponent : MonoBehaviour, IDamageable
{
    TankDataSO currentTank;
    [SerializeField] int numberOfHits;
    [SerializeField] GameObject explosionPrefab;

    private bool _isDead = false;
    private int currentHits;

    // Start is called before the first frame update
    void Start()
    {
        if (numberOfHits <= 0) numberOfHits = 1;
         currentHits = numberOfHits;

    }



    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        // 1. ПРЕДОХРАНИТЕЛЬ: Если танк уже мертв — игнорируем урон
        if (_isDead) return;

        // 2. ПРОВЕРКА ДРУЖЕСТВЕННОГО ОГНЯ (Friendly Fire):
        bool isPlayer = GetComponent<PlayerMove>() != null;

        if (isPlayer && isPlayerBullet)
        {
            return; // Пуля игрока попала в игрока — игнорируем
        }
        if (!isPlayer && !isPlayerBullet)
        {
            return; // Пуля врага попала во врага — игнорируем
        }

        // 3. Проверяем щит из TankBonusHandler
        TankBonusHandler bonusHandler = GetComponent<TankBonusHandler>();
        if (bonusHandler != null && bonusHandler.isInvulnereble)
        {
            return; // Щит активен — урон не проходит
        }

       
        currentHits -= damagePower;

        Debug.Log($"[УРОН] Объект {gameObject.name} ранен! Получено урона: {damagePower}. Осталось жизней: {currentHits}/{numberOfHits}");

        // Если урон огромный (граната), сразу принудительно обнуляем жизни
        if (damagePower > 50)
        {
            currentHits = 0;
        }

        // Вызываем смерть
        if (currentHits <= 0)
        {
            Die();
        }
    }


    public void InitHealth(int maxHits, TankDataSO data)
    {
        _isDead = false; 
        numberOfHits = maxHits;
        currentHits = maxHits;
        currentTank = data;
    }
    public void InitPlayerHealth(int maxHits)
    {
        numberOfHits = maxHits;
        currentHits = maxHits;
    }
    private void Die()
    {
        // если флаг уже взведен, не даем коду выполниться второй раз
        if (_isDead) return;
        _isDead = true; // Закрываем 

        EnemyMove enemyMove = GetComponent<EnemyMove>();
        BonusSpwn bonusSpwn = FindAnyObjectByType<BonusSpwn>();

        if (enemyMove != null)
        {
            EnemySpawn spawnManager = FindAnyObjectByType<EnemySpawn>();
            if (spawnManager != null)
            {
                spawnManager.RemoveDeadTank(gameObject);
            }

            // Бонусный танк
            BonusTankEffect bonusTank = GetComponent<BonusTankEffect>();
            if (bonusTank != null && bonusTank.isBonusTank && bonusSpwn != null)
            {
                 
               
                bonusSpwn.BonusInst();
            }
        }

        EnemyCounterUI counter = FindAnyObjectByType<EnemyCounterUI>();
        if (counter != null)
        {
            counter.OnEnemyDestroyed();
        }

        // Анимация взрыва
        if (explosionPrefab != null)
        {
           GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            Destroy(exp, 0.3f);
            
        }

        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.Shake(0.2f, 3f);
        }

        if (currentTank != null)
        {
            currentTank.OnDeathEvent?.Invoke(currentTank.scoreReward);
            Debug.Log($"[ОЧКИ] Событие смерти вызвано для {currentTank.name}. Очки за танк: {currentTank.scoreReward}");
        }
        
        // Ищем ScoreManager на сцене и передаем ему очки
        ScoreManager scoreManager = FindAnyObjectByType<ScoreManager>();
        if (scoreManager != null && currentTank != null)
        {
            scoreManager.AddScoreDirect(currentTank.scoreReward);
        }
        else if (scoreManager != null)
        {
            
            scoreManager.AddScoreDirect(100);
        }

        
        bool isPlayer = GetComponent<PlayerMove>() != null;
        if (isPlayer)
        {
            PlayerSpawnManager playerSpawn = FindAnyObjectByType<PlayerSpawnManager>();
            if (playerSpawn != null)
            {
                playerSpawn.Ressurecton(gameObject);
            }
        }

        gameObject.SetActive(false);

    }
    public void AddHealth(int amount)
    {
        currentHits += amount;
        Debug.Log($"[ЗДОРОВЬЕ] Объект {gameObject.name} получил прибавку к броне. Текущее здоровье: {currentHits}");
    }

}

