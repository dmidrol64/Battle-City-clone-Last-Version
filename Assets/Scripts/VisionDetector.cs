using System.Collections;
using UnityEngine;

public class VisionDetector : MonoBehaviour
{
    private EnemyMove enemy;
    private bool isRotating = false;

    [Header("Настройки сканирования")]
    [SerializeField] private float distance = 4f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float xSize = 1.5f; // подстрой под размер танка
    [SerializeField] private float ySize = 1.5f;

    [Header("Смещение радара вперёд")]
    [SerializeField] private float originOffset = 1.2f; // лучше небольшое смещение, а не 30

    void Start()
    {
        enemy = GetComponent<EnemyMove>();
    }

    void Update()
    {
        if (isRotating) return; // не запускаем проверку, пока идёт безопасный разворот
        Detect();
    }

    void Detect()
    {
        if (enemy == null) return;

        Vector2 size = new Vector2(xSize, ySize);
        float angle = transform.rotation.eulerAngles.z;
        Vector2 direction = transform.up;
        Vector2 rayOrigin = (Vector2)transform.position + direction * originOffset;

        RaycastHit2D hit = Physics2D.BoxCast(rayOrigin, size, angle, direction, distance, layerMask.value);

        if (hit.collider != null)
        {
            // Запускаем безопасный разворот только один раз, пока он идёт
            if (!isRotating)
            {
                StartCoroutine(SafeRotateRoutine());
            }
        }
    }

    private IEnumerator SafeRotateRoutine()
    {
        isRotating = true;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            isRotating = false;
            yield break;
        }

        // Останавливаем линейное и угловое движение
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Пытаемся найти направление без стены
        float currentAngle = transform.eulerAngles.z;
        float[] testAngles = { currentAngle + 90f, currentAngle - 90f }; // пробуем влево и вправо
        float chosenAngle = currentAngle;

        foreach (var angle in testAngles)
        {
            Quaternion testRot = Quaternion.Euler(0f, 0f, angle);
            Vector2 testDir = testRot * Vector2.up;
            Vector2 testOrigin = (Vector2)transform.position + testDir * originOffset;

            RaycastHit2D testHit = Physics2D.BoxCast(testOrigin, new Vector2(xSize, ySize), angle, testDir, distance, layerMask.value);
            if (testHit.collider == null)
            {
                chosenAngle = angle;
                break;
            }
        }

        // Плавно поворачиваем через физику
        Quaternion targetRot = Quaternion.Euler(0f, 0f, chosenAngle);
        float rotationSpeed = 10f; // подбери под свой геймплей

        while (Quaternion.Angle(transform.rotation, targetRot) > 5f)
        {
            rb.MoveRotation(Mathf.MoveTowardsAngle(rb.rotation, chosenAngle, rotationSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
        }

        // Финальная доводка угла
        rb.MoveRotation(chosenAngle);

        isRotating = false;
    }
}
