using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewVisionDetector : MonoBehaviour
{
    EnemyMove enemy;

    [SerializeField] float distance = 2f; // Длина лучей
    [SerializeField] LayerMask layerMask;

    BoxCollider2D boxCollider;
    Vector2[] _raycastPosition = new Vector2[3];
    Rigidbody2D _rb;
    RaycastHit2D[] _raycastHits = new RaycastHit2D[3];

    void Start()
    {
        enemy = GetComponent<EnemyMove>();
        boxCollider = GetComponent<BoxCollider2D>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckObstacle();
    }

    private void CheckObstacle()
    {
        if (boxCollider == null || enemy == null) return;

        if (enemy.isTurning) return;
        // Векторы направления, которые вращаются ВМЕСТЕ с танком
        Vector2 forward = transform.up;
        Vector2 rightSide = transform.right;

        // Точка старта лучей выдвинута на передний край коллайдера танка
        Vector2 centerOrigin = (Vector2)transform.position + (forward * (boxCollider.size.y * 0.5f));

        // Смещение в бока на половину ширины коллайдера
        float sideOffset = boxCollider.size.x * 0.6f;

        // Вычисляем локальные стартовые позиции для трех лучей
        _raycastPosition[0] = centerOrigin - (rightSide * sideOffset); // Левый ус
        _raycastPosition[1] = centerOrigin;                           // Центр
        _raycastPosition[2] = centerOrigin + (rightSide * sideOffset); // Правый ус

        int numberOfRays = 0;
        for (int i = 0; i < _raycastPosition.Length; i++)
        {
            // Пускаем луч  по ходу движения танка
           RaycastHit2D hit = Physics2D.Raycast(_raycastPosition[i], forward, distance, layerMask);

            // RaycastHit2D hit = Physics2D.Raycast(_raycastPosition[i], Vector2.down, distance, layerMask); 
            if (hit.collider != null)
            {
                _raycastHits[i] = hit;

                numberOfRays++;

                enemy.SideRotation();
                // enemy.transform.Rotate(0, 0, UnityEngine.Random.Range(1, 4) * 90);
                return;
             }
        }
        //if (hit.collider != null)
        //    {
        //        _raycastHits[i] = hit;
        //        enemy.SideRotation(i);
               
        //        return;
        //    }
        //    else
        //    {
        //        _raycastHits[i] = default;
        //    }
        //}
    }

    // Отрисовка красивых лучей во вкладке Scene
    private void OnDrawGizmos()
    {
        // Находим коллайдер прямо в режиме редактирования
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Vector2 forward = transform.up;
        Vector2 rightSide = transform.right;

        // Рассчитываем точки старта «усиков» прямо для Gizmos
        Vector2 centerOrigin = (Vector2)transform.position + (forward * (box.size.y * 0.5f));
        float sideOffset = box.size.x * 0.6f;

        Vector2[] positions = new Vector2[3];
        positions[0] = centerOrigin - (rightSide * sideOffset); // Левый
        positions[1] = centerOrigin;                           // Центр
        positions[2] = centerOrigin + (rightSide * sideOffset); // Правый

        for (int i = 0; i < positions.Length; i++)
        {
            // Если игра запущена и есть попадание — красим в красный, иначе в зеленый
            if (Application.isPlaying && _raycastHits != null && i < _raycastHits.Length && _raycastHits[i].collider != null)
            {
                Gizmos.color = Color.red;
            }
            else
            {
                Gizmos.color = Color.green;
            }

            Vector2 endPoint = positions[i] + (forward * distance);

            // Рисуем отладочную линию и сферу на конце
            Gizmos.DrawLine(positions[i], endPoint);
            Gizmos.DrawWireSphere(endPoint, 0.05f);
        }
    }

}
