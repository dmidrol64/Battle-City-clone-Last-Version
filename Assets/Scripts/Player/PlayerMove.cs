using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] public float speed;

    [Header("Звуки мотора")]
    [SerializeField] private AudioClip _idleSound;
    [SerializeField] private AudioClip _driveSound;

    private Vector2 _moveInput;
    private PlayerMovement _inputActions;
    private AudioSource _motorSounds;
    private Rigidbody2D _rb;

    public bool isFrozen = false;

    private void Awake()
    {
        _inputActions = new PlayerMovement();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _motorSounds = GetComponent<AudioSource>();

        if (_motorSounds != null && _idleSound != null)
        {
            _motorSounds.clip = _idleSound;
            _motorSounds.loop = true;
            _motorSounds.Play();
        }
    }

    void Update()
    {
        if (isFrozen)
        {
            _moveInput = Vector2.zero;
            return;
        }

        // Считываем вектор движения из нового Input System (WASD или стрелочки)
        _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();

        // Ограничиваем движение только по одной оси (запрещаем ехать по диагонали, как в классических Танчиках)
        if (_moveInput.x != 0)
        {
            _moveInput.y = 0;
        }

        
        bool isMoving = _moveInput != Vector2.zero;
        if (isMoving && _motorSounds.clip != _driveSound)
        {
            _motorSounds.clip = _driveSound;
            _motorSounds.loop = true;
            _motorSounds.Play();
        }
        else if (!isMoving && _motorSounds.clip != _idleSound)
        {
            _motorSounds.clip = _idleSound;
            _motorSounds.loop = true;
            _motorSounds.Play();
        }

        // стрельба по нажатию пробела
        if (_inputActions.Player.Fire.triggered)
        {
            ShootTank();
        }
    }

    private void FixedUpdate()
    {
        if (isFrozen)
        {
            _rb.linearVelocity = Vector2.zero;
            return;
        }

        // Движение через Rigidbody2D
        _rb.linearVelocity = _moveInput * speed;

        
        if (_moveInput.x < 0) // Влево
        {
            _rb.rotation = 90f;
        }
        else if (_moveInput.x > 0) // Вправо
        {
            _rb.rotation = -90f;
        }
        else if (_moveInput.y < 0) // Вниз
        {
            _rb.rotation = 180f;
        }
        else if (_moveInput.y > 0) // Вверх
        {
            _rb.rotation = 0f;
        }
    }

    private void ShootTank()
    {
        Shoot playerWeapon = GetComponent<Shoot>();

        if (playerWeapon == null)
        {
            playerWeapon = GetComponentInChildren<Shoot>();
        }

        if (playerWeapon != null)
        {
            playerWeapon.TryFire();
        }
        else
        {
            Debug.LogError($"[ОШИБКА ИГРОКА] Скрипт Shoot не найден ни на игроке, ни в его дочерних объектах!");
        }
    }
}
