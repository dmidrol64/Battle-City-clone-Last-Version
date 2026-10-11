using System.Collections;
using UnityEngine;

public class BonusTankEffect : MonoBehaviour
{
    public bool isBonusTank = false;

    private SpriteRenderer _blinkColor;
    private Coroutine _blinkCoroutine;

    private void Awake()
    {
        // Находим картинку танка в дочерних объектах
        _blinkColor = GetComponentInChildren<SpriteRenderer>();
    }
    // Срабатывает каждый раз, когда танк извлекается из пула объектов
    private void OnEnable()
    {
        // Первым делом жестко сбрасываем цвет в стандартный белый
        ResetColor();
    }

    // Срабатывает, когда танк возвращается на склад пула (умирает)
    private void OnDisable()
    {
        // Принудительно тушим корутину, чтобы цвета не накладывались в будущем
        StopBlinking();
    }

    public void InitBonusStatus(bool isBonus)
    {
        isBonusTank = isBonus;
        // Если танк назначен бонусным — запускаем мерцание
        if (gameObject.activeInHierarchy && isBonusTank)
        { // На всякий случай останавливаем корутину что бы не сработала дважды
            StopBlinking();
            _blinkCoroutine = StartCoroutine(BlinkRoutine());
        }
        else
        {
            // Если обычный — тушим мерцание и сбрасываем цвет
            StopBlinking();
            ResetColor();
        }

    }

    private IEnumerator BlinkRoutine()
    {
        // Цикл работает, пока танк живой и имеет статус бонусного
        while (isBonusTank && _blinkColor != null)
        {
            _blinkColor.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            _blinkColor.color = Color.white;
            yield return new WaitForSeconds(0.2f);
        }
    }

    private void StopBlinking()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;
        }
    }

    private void ResetColor()
    {
        if (_blinkColor == null)
        {
            _blinkColor = GetComponentInChildren<SpriteRenderer>();
        }

        if (_blinkColor != null)
        {
            _blinkColor.color = Color.white;
        }
    }
}
