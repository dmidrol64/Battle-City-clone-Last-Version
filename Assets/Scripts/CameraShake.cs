using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Статическая ссылка, чтобы любой скрипт мог вызвать тряску в 1 строчку
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;

    private void Awake()
    {
        // Настраиваем синглтон для быстрого доступа
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Запоминаем каноничное положение камеры (наша Z = -10)
        originalPosition = transform.localPosition;
    }

    // Главный метод запуска тряски
    public void Shake(float duration, float magnitude)
    {
        // Запускаем корутину тряски, предварительно остановив прошлую (если танки взрываются одновременно)
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Генерируем случайное микро-смещение по осям X и Y
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Сдвигаем камеру, сохраняя нашу плоскость Z = -10!
            transform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null; // Ждем один кадр
        }

        // Время вышло — железно возвращаем камеру в исходное ровное положение
        transform.localPosition = originalPosition;
    }
}
