using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoot : MonoBehaviour
{
    [Header("Префабы")]
    [SerializeField] GameObject firePrefab;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] public Transform bulletPlace;

    [Header("Настройки перезарядки")]
    [SerializeField] public float rate = 0.5f;
    AudioSource bulletSourse;
    List<GameObject> sleepBullets = new List<GameObject>();
    private float currentRate;

    private int countBullet = 20;
    private float bulletSpeed;
    public int levelWeapon = 0;

    private PlayerMove playerMove;
    private void OnEnable()
    {
        bulletSourse = GetComponentInChildren<AudioSource>();
    }
    void Start()
    {
        // Ищем скрипт игрока на родительском контейнере, чтобы понять, чья это пушка
        playerMove = GetComponentInParent<PlayerMove>();
        
        if (bulletPlace == null) bulletPlace = transform;
        currentRate = rate;
        CreateBullet();
    }

    void Update()
    {
        // Очищаем уничтоженные пули из списка активных
        sleepBullets.RemoveAll(b => b == null);

        // Плавно уменьшаем таймер перезарядки
        if (currentRate > 0)
        {
            currentRate -= Time.deltaTime;
        }

        
        // Если пушка врага (playerMove равен null), ИИ заморожен и пули на экране закончились
        if (playerMove == null && currentRate <= 0 && EnemySpawn.IsTimeFrozen == false && GetActiveBulletsCount() < 1)
        {
            bulletSpeed = 20f; // Обычная скорость вражеской пули
            GetBullet();
            currentRate = rate; // Сбрасываем перезарядку
        }
    }

   
    public void TryFire()
    {
        // Проверяем таймер перезарядки и лимиты пуль на экране
        if (currentRate <= 0 && CanPlayerShoot())
        {
            // Скорость пули увеличивается, если оружие прокачано бонусами
            bulletSpeed = (levelWeapon >= 1) ? 50f : 20f;

            GetBullet();

            // Сбрасываем перезарядку
            currentRate = rate;
        }
    }
    private int GetActiveBulletsCount()
    {
        int activeCount = 0;
        //одна пуля
        for (int i = 0; i < sleepBullets.Count; i++)
        {
            if (sleepBullets[i] != null && sleepBullets[i].activeInHierarchy)
            {
                activeCount++;
            }
        }
        return activeCount;
    }
    private bool CanPlayerShoot()
    {
        int activeBullets = GetActiveBulletsCount();
        // На 0 и 1 уровне оружия игрок может выпустить только 1 пулю за раз
        if (levelWeapon < 2 && activeBullets < 1) return true;

        // На 2 и 3 уровне оружия (Пистолет) игрок может выпустить 2 пули одновременно
        if (levelWeapon >= 2 && activeBullets < 2) return true;

        return false;
    }

    private GameObject GetBullet()
    {
        for (int i = 0; i < sleepBullets.Count; i++)
        { 
            if(sleepBullets[i].activeInHierarchy == false)
            {
                GameObject poolingBullets = sleepBullets[i];
                poolingBullets.transform.SetParent(null);
                poolingBullets.transform.position = bulletPlace.position;
                poolingBullets.transform.rotation = bulletPlace.rotation;
                poolingBullets.SetActive(true);

                Rigidbody2D bulletRb = poolingBullets.GetComponent<Rigidbody2D>();
                if (bulletRb != null)
                {
                    bulletRb.linearVelocity = bulletPlace.up * bulletSpeed;
                }

                if(poolingBullets != null)
                {
                    bulletSourse.Play();
                }
                else
                {
                    bulletSourse.Stop();
                }
                StartCoroutine(Fire());
                
                BulletCollision bulletScript = poolingBullets.GetComponent<BulletCollision>();
                if (bulletScript != null)
                {
                    
                    bulletScript.bulletPower = 1 + levelWeapon;

                    bulletScript.isPlayerBullet = (playerMove != null);
                }

                // Выставляем правильные физические слои
                if (playerMove != null)
                {
                    poolingBullets.layer = LayerMask.NameToLayer("PlayerBullet");
                }
                else
                {
                    poolingBullets.layer = LayerMask.NameToLayer("EnemyBullet");
                }

                // Передаем коллайдер контейнера танка в пулю для мгновенного авто-игнорирования
                Collider2D tankCollider = GetComponent<Collider2D>();
                if (bulletScript != null && tankCollider != null)
                {
                    bulletScript.InitBullet(tankCollider);
                }
                return poolingBullets;
            }
            
        }
        return null;
    }

   private IEnumerator Fire()
    {
        //анимация выстрела
        if (firePrefab != null)
        {
          GameObject fireClone =  Instantiate(firePrefab, bulletPlace.position, transform.rotation);

            Destroy(fireClone, 0.15f);
        }
        yield return new WaitForSeconds(0.2f);
    }

    void CreateBullet()
    {
        if (bulletPrefab == null || bulletPlace == null) return;

       for(int i = 0; i < countBullet; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform);
            bullet.SetActive(false);

            sleepBullets.Add(bullet);
        }
 
    }

    public void InitWeapon(TankDataSO data)
    {
        levelWeapon = data.weaponLevel;
        rate = data.fireRate;
    }
}
