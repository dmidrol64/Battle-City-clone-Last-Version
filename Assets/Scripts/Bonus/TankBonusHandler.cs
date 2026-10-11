using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankBonusHandler : MonoBehaviour,IDamageable
{

    [SerializeField] private GameObject shieldPrefab;
    [SerializeField] private AudioClip bonusPickUpSound;
    private AudioSource audioSource;
    private GameObject currentShieldInstance;
    EnemySpawn enemySpawn;

   public bool isInvulnereble = false;
   private bool isPlayer;

    private void Awake()
    {
        isPlayer = GetComponent<PlayerMove>() != null;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void OnDisable()
    {
        isInvulnereble = false;

        // Если танк ушел в пул с активным щитом — принудительно удаляем его, чтобы не переносить в новый спавн
        if (currentShieldInstance != null)
        {
            Destroy(currentShieldInstance);
            currentShieldInstance = null;
        }

        // Останавливаем все корутины этого объекта, чтобы таймеры щитов не наслаивались
        StopAllCoroutines();
    }
    public void ActivateBonus(BonusType type)
    {
        Debug.Log($"[ТЕСТ БОНУСА] Объект {gameObject.name} активировал бонус типа: {type}");
        enemySpawn = FindAnyObjectByType<EnemySpawn>();

        if (audioSource != null && bonusPickUpSound != null)
        {
            audioSource.PlayOneShot(bonusPickUpSound);
        }

        switch (type) {

            case BonusType.Star:
                UpgradeWeapon();
                break;


            case BonusType.Helmet:
               StartCoroutine(GiveShield());
                break;


            case BonusType.Pistol:
                MakeMaxLevel();
                break;

            case BonusType.Tank:
                ArmorTank();
                break;

            case BonusType.Shovel:
                ArmorBase();
                break;

            case BonusType.Timer:
                StartCoroutine(Freeze());
                break;

            case BonusType.Granade:
                Exploid();
                break;
        }
    }


    private void UpgradeWeapon()
    {
        Shoot weapon = GetComponentInChildren<Shoot>();
        if (weapon != null)
        {
            if (weapon.levelWeapon < 3)
            {
                weapon.levelWeapon++;

                Debug.Log($"{gameObject.name} повысил уровень оружия до: {weapon.levelWeapon}");
            }
        }
    }

    public IEnumerator GiveShield()
    {
        // 1. Включаем бессмертие именно этому танку
        isInvulnereble = true;

        // 2. Если префаб анимации щита настроен — спавним его дочерним элементом!
        if (shieldPrefab != null && currentShieldInstance == null)
        {
            // transform — это сам танк, привязываем анимацию к нему
            currentShieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);

            // Центрируем мерцающий круг строго по середине танка
            currentShieldInstance.transform.localPosition = Vector3.zero;
        }

        // 3. Танк бессмертен 5 секунд (или выставь больше, если нужно)
        yield return new WaitForSeconds(5f);

        // 4. Время вышло — отключаем защиту и удаляем анимацию из памяти
        isInvulnereble = false;

        if (currentShieldInstance != null)
        {
            Destroy(currentShieldInstance);
            currentShieldInstance = null; // Очищаем ссылку
        }
    }

    private void MakeMaxLevel()
    {
        Shoot weapon = GetComponentInChildren<Shoot>();
        if (weapon != null)
        {
            weapon.levelWeapon = 4;
            Debug.Log($"{gameObject.name} получил МАКСИМАЛЬНЫЙ уровень оружия от Пистолета!");
        }
    }

    private void ArmorBase()
    {
        WallsAroundBase aroundBase = FindAnyObjectByType<WallsAroundBase>();
        if (isPlayer)
        {
            
            aroundBase.PickUpShovelBonus();
        }
        else
        {
            aroundBase.EnemyPickUpShovelBonus();
        }
       
    }
    private IEnumerator Freeze()
    {
        if (isPlayer)
        {
            EnemySpawn.IsTimeFrozen = true;
            for (int i = 0; i < enemySpawn.activeTank.Count; i++)
            {
                enemySpawn.activeTank[i].GetComponent<EnemyMove>().FreezeTank();

            }

            yield return new WaitForSeconds(5f);
            for (int i = 0; i < enemySpawn.activeTank.Count; i++)
            {
                enemySpawn.activeTank[i].GetComponent<EnemyMove>().UnfreezeTank();
            }
            EnemySpawn.IsTimeFrozen = false;
        }
        else
        {
            //  если бонус взял ВРАГ, застывает Игрок!
            PlayerMove player = FindAnyObjectByType<PlayerMove>();
            if (player != null)
            {
                player.isFrozen = true; // Заколдовали игрока
            }

            yield return new WaitForSeconds(5f); // Игрок стоит беспомощным 5 секунд

            // Пытаемся снова найти игрока (вдруг его убили пока он стоял, и спавнился новый)
            PlayerMove regeneratedPlayer = FindAnyObjectByType<PlayerMove>();
            if (regeneratedPlayer != null)
            {
                regeneratedPlayer.isFrozen = false; // Разморозили игрока
            }
        }
    
    }

    private void Exploid()
    {
        if (isPlayer)
        {
            for (int i = enemySpawn.activeTank.Count - 1; i >= 0; i--)
            {
                if (enemySpawn.activeTank[i] != null)
                {
                    IDamageable damageable = enemySpawn.activeTank[i].GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        // Наносим огромный смертельный урон! 
                        // Танк сам вызовет свой метод Die(), красиво взорвется и удалится из списков спавнера!
                        damageable.TakeDamage(99, true, transform.position);
                    }
                }

            }
        }
        else
        {
            PlayerMove activePlayer = FindAnyObjectByType<PlayerMove>();
            PlayerSpawnManager playerSpawn = FindAnyObjectByType<PlayerSpawnManager>();
            if (playerSpawn != null && activePlayer != null)
            {
                playerSpawn.Ressurecton(activePlayer.gameObject);
            }
        }
        
    }

    private void ArmorTank()
    {
        if (isPlayer)
        {
            PlayerSpawnManager manager = FindAnyObjectByType<PlayerSpawnManager>();
            manager.currentLives++;
            Debug.Log(manager.currentLives);
        }
        else
        {
            for (int i = 0; i < enemySpawn.activeTank.Count; i++)
            {
                if(enemySpawn.activeTank[i] != null)
                {
                    LivesComponent tankHealt = GetComponent<LivesComponent>();
                    if(tankHealt != null)
                    {
                        tankHealt.AddHealth(1);
                    }
                }
            }
        }
    }

    public void ActivateSpawnShield()
    {
        if (isPlayer)
        {
            StartCoroutine(GiveShield());
        }
        else
        {

        }
    }
    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        // ТВОЯ РОДНАЯ ЛОГИКА:
        if (isPlayerBullet == true)
        {
            return; // Если пуля игрока — игнорируем
        }
        if (isInvulnereble == true)
        {
            // Если танк бессмертен (горит щит) — урон не проходит
            return;
        }

        // Логика респауна игрока
        PlayerSpawnManager playerSpawn = FindAnyObjectByType<PlayerSpawnManager>();
        if (playerSpawn != null)
        {
            playerSpawn.Ressurecton(gameObject);
        }
    }


}
