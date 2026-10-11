using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelDamageController : MonoBehaviour, IDamageable
{
    [Header("Эффекты частиц")]
    [SerializeField] private GameObject debrisParticlePrefab; // Для кирпичей (Bricks)
    [SerializeField] private GameObject steelParticlePrefab;  // Для стали (Steel)

    private Tilemap[] _allTilemaps;

    private void Awake()
    {
        // Находим все тайлмапы на сцене 
        _allTilemaps = FindObjectsByType<Tilemap>(FindObjectsInactive.Exclude);
    }

    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        if (_allTilemaps == null || _allTilemaps.Length == 0) return;

        // Массив микро-шагов вокруг точки удара (саму клетку и соседние)
        int[] steps = { -1, 0, 1 };

        foreach (var tm in _allTilemaps)
        {
            //  Если пуля слабее 3-го уровня (не Пистолет), сталь полностью игнорирует её
            if (tm.name.Contains("Steel"))
            {
                // Если сила пули равна 1 или 2 — сталь не пробивается, уходим на следующий слой
                if (damagePower == 1 || damagePower == 2)
                {
                    continue;
                }
            }

            // Если это не кирпичи и не сталь — пропускаем этот слой
            if (!tm.name.Contains("Bricks") && !tm.name.Contains("Steel"))
            {
                continue;
            }

            // Переводим точку взрыва в локальные координаты тайлмапа
            Vector3 localPos = tm.transform.InverseTransformPoint(hitPosition);
            Vector3Int cellPosition = tm.WorldToCell(localPos);

            // Безопасный перебор ячеек без ломающихся символов
            foreach (int x in steps)
            {
                foreach (int y in steps)
                {
                    Vector3Int targetCell = cellPosition + new Vector3Int(x, y, 0);

                    // Если в этой ячейке нашли тайл — мгновенно стираем его!
                    if (tm.HasTile(targetCell))
                    {
                        Debug.Log($"[LevelDamage] УСПЕХ! Разрушен блок на слое '{tm.name}' в клетке: {targetCell}");
                        DestroyTileAt(tm, targetCell);
                        return; // Блок удален, выходим из метода
                    }
                }
            }
        }
    }

    private void DestroyTileAt(Tilemap tilemap, Vector3Int cell)
    {
        // Находим центр клетки для красивого спавна осколков
        Vector3 centerWorld = tilemap.GetCellCenterWorld(cell);

        Vector3 effectPosition = new Vector3(centerWorld.x, centerWorld.y, -2f);

        //  Смотрим, на каком слое удалили блок
        GameObject activeEffect = debrisParticlePrefab; // По дефолту кирпич

        if (tilemap.name.Contains("Steel"))
        {
            activeEffect = steelParticlePrefab; // Если стерли сталь — берем искры стали
        }

        // Спавним выбранные частицы взрыва
        if (activeEffect != null)
        {
            GameObject p = Instantiate(activeEffect, effectPosition, Quaternion.identity);
            p.transform.localScale = Vector3.one;

            // Дополнительно выталкиваем на передний план рендерера
            var psRenderer = p.GetComponent<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                psRenderer.sortingOrder = 100;
            }

           // Destroy(p, 1f);

            Destroy(p, 1f); // Удаляем объект эффекта через 1 секунду
        }

        // СТИРАЕМ ТАЙЛ ИЗ СЕТКИ КАРТЫ!
        tilemap.SetTile(cell, null);

        // Принудительно заставляем обновить физические коллайдеры
        var tc = tilemap.GetComponent<TilemapCollider2D>();
        if (tc != null) tc.ProcessTilemapChanges();

        var cc = tilemap.GetComponent<CompositeCollider2D>();
        if (cc != null) cc.GenerateGeometry();
    }
}
