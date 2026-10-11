using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BrickWall : MonoBehaviour, IDamageable
{
    private Tilemap _tileMap;

    [Header("Эффекты разрушения")]
    [SerializeField] public GameObject debrisParticlePrefab;

    private void Awake()
    {
        _tileMap = GetComponent<Tilemap>() ?? GetComponentInChildren<Tilemap>();

        if (_tileMap == null)
        {
            Debug.LogError($"[BrickWall] Не найден Tilemap на объекте {name} или его детях! Проверь иерархию.");
        }
    }
    private void Start()
    {
        
    }

    // МЕТОД ИНТЕРФЕЙСА: теперь принимает точную позицию пули напрямую!
    public void TakeDamage(int damagePower, bool isPlayerBullet, Vector3 hitPosition)
    {
        if (_tileMap == null) return;

        // Берем направление "вверх" по умолчанию (или можно передавать из пули, 
        // но для ровных клеток Battle City достаточно просто зайти на 0.1 единицы внутрь)
        //Vector3 correctedPosition = hitPosition + (Vector3.up * 0.1f);

        // В Unity 2D пуля летит в разные стороны, поэтому надежнее всего 
        // брать саму точку hitPosition, так как пуля-триггер уже находится внутри коллайдера клетки!
        Vector3Int cellPosition = _tileMap.WorldToCell(hitPosition);

        // Проверяем: если в этой клетке сетки действительно есть кирпич — стираем его!
        if (_tileMap.HasTile(cellPosition))
        {
            Vector3 tileCenterWorld = _tileMap.GetCellCenterWorld(cellPosition);
            SpawnDebris(tileCenterWorld);
            _tileMap.SetTile(cellPosition, null);
        }
    }

    private void SpawnDebris(Vector3 spawnPos)
    {
        if (debrisParticlePrefab != null)
        {
            GameObject particleInstance = Instantiate(debrisParticlePrefab, spawnPos, Quaternion.identity);
            ParticleSystem ps = particleInstance.GetComponent<ParticleSystem>();
            if (ps != null) ps.Play();
            Destroy(particleInstance, 1f);
        }
    }
}
