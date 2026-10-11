using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Эта строчка позволяет создавать файлы бонусов прямо через контекстное меню мыши в Unity!
[CreateAssetMenu(fileName = "NewBonusData", menuName = "BattleCity/Bonus Data")]
public class BonusDataSO :ScriptableObject
{

    [Header("Базовые настройки")]
    public BonusType bonusType;       // Тип (Star, Helmet, Timer...)
    public GameObject visualPrefab;       // Картинка, которая отобразится на карте
    public int scoreReward = 500;     // Сколько очков дают за подбор

    [Header("Тайминги")]
    public float lifetime = 10f;      // Сколько секунд бонус лежит на земле, пока не исчезнет
    public float effectDuration = 10f; // Время действия эффекта (для Часов, Каски, Лопаты)
}
