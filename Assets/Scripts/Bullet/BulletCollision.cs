using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static IDamageable;

public class BulletCollision : MonoBehaviour
{
    [Header("Характеристики пули")]
    public int bulletPower = 1; // Уровень оружия (передается из Shoot.cs)
    public bool isPlayerBullet; // Кто выстрелил: true - Игрок, false - Враг


    private void OnEnable()
    {
        // Если за 4 секунды пуля ни во что не попала (улетела за экран), выключаем её
        Invoke(nameof(DeactivateBullet), 4f);
    }

    private void OnDisable()
    {
        // Обязательно отменяем таймер, если пуля попала в цель раньше, чем через 4 секунды
        CancelInvoke(nameof(DeactivateBullet));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[Bullet] Попал в объект: '{collision.gameObject.name}', " +
              $"parent: '{collision.transform.parent?.name}', " +
              $"тип коллайдера: {collision.GetType().Name}");
        Debug.Log($"[Bullet] Позиция пули: {transform.position:F2}");

        // 1. Пуля летит сквозь воду без препятствий
        if (collision.gameObject.GetComponent<Water>() != null)
        {
            return;
        }

        
        IDamageable damageableObject = collision.gameObject.GetComponentInParent<IDamageable>();

        if (damageableObject != null)
        {
            Debug.Log($"[ФИЗИКА ПУЛИ] Попадание в цель! Наношу урон: {bulletPower}, Пуля игрока: {isPlayerBullet}");

            Vector3 correctedPos = transform.position + (transform.up * 0.1f);
            damageableObject.TakeDamage(bulletPower, isPlayerBullet, correctedPos);

            DeactivateBullet();
            return;
        }

        // 3. СТАЛЬНЫЕ БЛОКИ
        if (collision.gameObject.GetComponent<SteelWall>() != null || collision.gameObject.name.Contains("Steel"))
        {
            Debug.Log($"[ФИЗИКА ПУЛИ] Врезался в сталь! Сила пули: {bulletPower}");

            if (bulletPower >= 3)
            {
                Destroy(collision.gameObject);
            }

            DeactivateBullet();
            return;
        }

        // 4. КРАЙНЯЯ СТРАХОВКА
        DeactivateBullet();
    }

    public void InitBullet(Collider2D tankCollider)
    { // Достаем собственный коллайдер пули
      Collider2D bulletCollider = GetComponent<Collider2D>();
        // Если оба коллайдера существуют — принудительно запрещаем им сталкиваться
        if (bulletCollider != null && tankCollider != null)
        {
            Physics2D.IgnoreCollision(bulletCollider, tankCollider, true);
        }
    }

   
    private void DeactivateBullet()
    {
        gameObject.SetActive(false);
    }
}
