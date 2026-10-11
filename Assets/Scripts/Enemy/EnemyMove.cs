using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyMove : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 360f; // Скорость поворота (градусы в секунду)
    [SerializeField] public float enemySpeedMoving;
    [SerializeField] AudioClip _eDrive;
    [SerializeField] private Transform visionPoint;
    private bool isFrozen = false;
    public float baseSpeed;

    private Quaternion targetRotation;
    private AudioSource _enemyDrive;
    private Rigidbody2D _rb;

   
    private float targetAngle; // Угол, к которому мы плавно поворачиваемся

    public bool isTurning = false;

    private LayerMask layerMask;

    private void Awake()
    {
        
        _rb = GetComponent<Rigidbody2D>();
        _enemyDrive = GetComponent<AudioSource>();

        int bonusLayer = LayerMask.NameToLayer("Bonus");
        layerMask = ~(
            LayerMask.GetMask("Enemy", "Player", "PlayerBullet", "EnemyBullet") |
            (1 << bonusLayer)
        );
    }

  // Каждый раз при выходе танка со склада
    private void OnEnable()
    {
        if (EnemySpawn.IsTimeFrozen) FreezeTank();
        else UnfreezeTank();

        isTurning = false; // Сбрасываем флаг разворота

    }
    private void OnDisable()
    {
        isTurning = false; // Железный сброс для пула
        StopAllCoroutines();
    }


    void FixedUpdate()
    {
        if (isFrozen || isTurning)
        {
            _rb.linearVelocity = Vector2.zero;
            if (_enemyDrive != null) _enemyDrive.Stop();
            return;
        }

        _rb.linearVelocity = transform.up * baseSpeed;
        HandleAudio(baseSpeed);
    }

    public void SideRotation()
    {
        if (isTurning) return;

        

        //// Сбрасываем физическую скорость, чтобы танк не ехал юзом во время вращения
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        //// Запоминаем текущий ровный поворот танка
        Quaternion currentRot = transform.rotation;
        float[] smartAngles = { 90f, -90f, 180f };
        float chosenAngle = smartAngles[Random.Range(0, smartAngles.Length)];
        targetRotation = currentRot * Quaternion.Euler(0f, 0f, chosenAngle);


        //if (side == 0)
        //{
        //    // Препятствие СЛЕВА -> плавно поворачиваем ВПРАВО (-90 градусов по оси Z)
        //    targetRotation = currentRot * Quaternion.Euler(0f, 0f, -90f);
        //    Debug.Log("[ИИ] Плавный Slerp-поворот вправо.");
        //}
        //else if (side == 2)
        //{
        //    // Препятствие СПРАВА -> плавно поворачиваем ВЛЕВО (+90 градусов по оси Z)
        //    targetRotation = currentRot * Quaternion.Euler(0f, 0f, 90f);
        //    Debug.Log("[ИИ] Плавный Slerp-поворот влево.");
        //}
        //else if (side == 1)
        //{
        //    // Препятствие ПО ЦЕНТРУ -> выбираем случайный ровный разворот
        //    float[] smartAngles = { 90f, -90f, 180f };
        //    float chosenAngle = smartAngles[Random.Range(0, smartAngles.Length)];
        //    targetRotation = currentRot * Quaternion.Euler(0f, 0f, chosenAngle);
        //    Debug.Log("[ИИ] Плавный Slerp-разворот по центру.");
        //}

        StartCoroutine(SmoothTurnRoutine());
    }



    private void HandleAudio(float speed)
    {
        if (_enemyDrive != null && _eDrive != null)
        {
            if (speed > 0 && _enemyDrive.clip != _eDrive)
            {
                _enemyDrive.clip = _eDrive;
                _enemyDrive.loop = true;
                _enemyDrive.Play();
            }
            else if (speed == 0)
            {
                _enemyDrive.Stop();
            }
        }
    }

    public void FreezeTank() { isFrozen = true; }
    public void UnfreezeTank() { isFrozen = false; }


    public IEnumerator SmoothTurnRoutine()
    {
        isTurning = true;

        // Зануляем физику еще раз для безопасности
        _rb.linearVelocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        // Настройка чувствительности вращения (чем выше число, тем быстрее танк повернется)
        float speedModifier = 10f;

        while (true)
        {
            // Плавно приближаем текущий поворот танка к целевому через Slerp
            // Использование Time.fixedDeltaTime обязательно для синхронизации с физикой
            Quaternion nextRotation = Quaternion.Slerp(transform.rotation, targetRotation, speedModifier * Time.fixedDeltaTime);

            // Применяем поворот к Rigidbody2D
            _rb.MoveRotation(nextRotation);

            // Проверяем: если угол между текущим и целевым поворотом стал меньше 0.5 градуса
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.5f)
            {
                _rb.MoveRotation(targetRotation); // Жестко фиксируем строго перпендикулярный угол (0, 90, 180, 270)
                break;
            }

            yield return new WaitForFixedUpdate();
        }

        isTurning = false; // Поворот идеально завершен, включаем мотор для езды по прямой!
    }

    // Метод настройки параметров из ScriptableObject
    public void InitTank(TankDataSO tankData)
    {
        enemySpeedMoving = tankData.moveSpeed;
        baseSpeed = enemySpeedMoving;
       
    }
}
