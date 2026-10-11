using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    GameObject handlerObject;
    PlayerRessurection playerHP;
    TankBonusHandler handler;

     [SerializeField] PlayerDataSO playerData;
    [SerializeField] GameObject explosionPrefab;
    private int currentHits;
    public int currentLives;

    Vector3 spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (playerData == null)
        {
            Debug.LogError("[КРИТИЧЕСКАЯ ОШИБКА] Не назначен паспорт playerData в PlayerSpawnManager!");
            return;
        }

        currentLives = playerData.globalLives;
        currentHits = playerData.hits;

        PlayerSpawn();
    }



    public void PlayerSpawn()
    {

        spawnPosition = new Vector3(-7f, -13f, 0);


        handlerObject = Instantiate(playerData.BasePlayerContainer, spawnPosition, Quaternion.identity);


        if (handlerObject != null)
        {
            // ЗАПУСКАЕМ НАШ КОНСТРУКТОР:
            // Инициализатор сам настроит скорость, жизни пушки и заспавнит графику внутрь!
            PlayerInitializer initializer = handlerObject.GetComponent<PlayerInitializer>();
            if (initializer != null)
            {
                initializer.SetupPlayer(playerData);
            }
            else
            {
                Debug.LogWarning("[ВНИМАНИЕ] На префабе игрока не найден скрипт PlayerInitializer. Добавь его!");
            }
            // Включаем бессмертие при появлении (щит спавна)
            TankBonusHandler tankBonus = handlerObject.GetComponent<TankBonusHandler>();
            if (tankBonus != null)
            {
                tankBonus.ActivateSpawnShield();
            }

        }
    }

    public void Ressurecton(GameObject DeadPlayerTank)
    {
        currentHits--;

        if (currentHits <= 0)
        {
            if (DeadPlayerTank != null)
            {
               GameObject playerExplosion = Instantiate(explosionPrefab,DeadPlayerTank.transform.position, Quaternion.identity);
                Destroy(playerExplosion, 0.4f);
                Destroy(DeadPlayerTank);
            }

            if (currentLives > 1)
            {
                // Есть ещё жизни — респаун
                currentLives--;
                

                PlayerSpawn();

                Debug.Log($"Респаун! Осталось жизней: {currentLives}");
            }
            else
            {
                // Жизней больше нет — Game Over
                currentLives = 0;
                currentHits = 0;

                Debug.Log("GAME OVER — жизни закончились!");

                // Ищем менеджер проигрыша и показываем экран
                var gameOverMgr = FindAnyObjectByType<GameOverManager>();
                if (gameOverMgr != null)
                {
                    gameOverMgr.ShowGameOver();
                }
                else
                {
                    Debug.LogError("[Ressurecton] Не найден GameOverManager на сцене!");
                }
            }
        }
    }




}
