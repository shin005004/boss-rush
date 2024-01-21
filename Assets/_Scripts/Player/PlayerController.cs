using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayerController
{
    #region INTERNAL
    private PlayerInput _input;
    private FrameInput _frameInput;

    private int _playerHP;

    private bool _hasControl;
    #endregion

    #region EXTERNAL
    public PlayerStatSO PlayerStats;

    public event Action<bool, Vector2> RollingChanged;

    public event Action<Vector2> Attacked;
    public event Action AttackEnd;

    public event Action<bool> ShootStanceChanged;
    public event Action<bool, Vector2> Shotted;

    public Vector2 MousePosition { get; set; }
    public Vector2 PlayerInput => currentPlayerDirection;
    public Vector2 PlayerDirection => cachedPlayerDirection;

    public bool IsRolling { get; private set; }
    #endregion

    #region SETUP
    private void Start()
    {
        _input = GetComponent<PlayerInput>();

        // Setup
        _playerSpeed = PlayerStats.PlayerSpeed;
        _rollMovementModifier = PlayerStats.RollMovementModifier;

        _rollStartVelocity = PlayerStats.RollStartVelocity;
        _rollLengthTime = PlayerStats.RollLengthTime;
        _rollCoolTime = PlayerStats.RollCoolTime;
        _rollStartUp = PlayerStats.RollStartUp;
        _rollInvlunTime = PlayerStats.RollInvlunTime;

        _justRollPrefab = PlayerStats.JustRollPrefab;
    }
    #endregion

    #region UPDATE
    private void Update()
    {
        GatherInput();
        HandlePlayerFlipping();

        HandleMoving();
        HandleRolling();
    }
    #endregion

    #region INPUT
    private void GatherInput()
    {
        _frameInput = _input.FrameInput;
        MousePosition = (Vector2)_frameInput.MousePosition;

        if (_frameInput.RollDown && !IsRolling) rollToConsume = true;
    }
    #endregion

    #region MOVING
    private float _playerSpeed;
    private float _rollMovementModifier;

    private bool canMove = true;
    private Vector2 currentPlayerDirection = new Vector3(0f, 0f, 0f);
    private Vector2 cachedPlayerDirection = new Vector2(0f, 0f);

    private void HandleMoving()
    {
        if (_frameInput.Move.x != 0 || _frameInput.Move.y != 0)
        {
            currentPlayerDirection = new Vector3(_frameInput.Move.x, _frameInput.Move.y).normalized;
            cachedPlayerDirection = currentPlayerDirection;
        }
        else
            currentPlayerDirection = Vector3.zero;

        var posX = transform.position.x + (_playerSpeed / _rollMovementModifier) * currentPlayerDirection.x * Time.deltaTime;
        var posY = transform.position.y + (_playerSpeed / _rollMovementModifier) * currentPlayerDirection.y * Time.deltaTime;
        transform.position = new Vector3(posX, posY, 0f);
    }

    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 flippedScale = new Vector3(-1f, 1f, 1f);
    private void HandlePlayerFlipping()
    {
        // Debug.Log(_playerController.PlayerInput);
        if (Mathf.Abs(PlayerInput.x) > 0.1f)
        {
            if (PlayerInput.x < 0)
                transform.localScale = flippedScale;
            else
                transform.localScale = normalScale;
        }
    }
    #endregion

    #region ROLLING
    private float _rollStartVelocity;   // 구르기 시작 속도
    private float _rollLengthTime;      // 전체 구르기 시간
    private float _rollCoolTime;        // 구르기 시작부터 쿨다운

    private float _rollStartUp;
    private float _rollInvlunTime;

    private bool canRoll = true;            // 외적으로 구르기 가능한지?
    private bool rollToConsume = false;     // 대쉬 입력이 들어왔는가?
    private bool rollCoolDownFlag = true;   // 쿨다운으로 구르기 가능한지?

    // 구르기 시간 확인하는 시간
    private float rollElapsedTime = 0f;

    // 저스트 구르기 확인용
    private GameObject _justRollPrefab;
    private bool isRollSuccessFlag = false;

    private void HandleRolling()
    {
        if (!rollToConsume) return;

        if (canRoll && canMove && rollCoolDownFlag)
        {
            rollCoolDownFlag = false;
            RollingChanged?.Invoke(true, cachedPlayerDirection);
            StartCoroutine(Roll());
        }

        rollToConsume = false;
    }

    private void ResetRoll()
    {
        rollCoolDownFlag = true;
    }

    private IEnumerator Roll()
    {
        GameObject rollDummyObject = Instantiate(_justRollPrefab, transform.position, Quaternion.identity);
        JustRollDummy rollDummyComponent = rollDummyObject.GetComponent<JustRollDummy>();
        rollDummyComponent.StartUp(_rollStartUp, _rollInvlunTime, this);

        Vector3 rollDirection = new Vector3(cachedPlayerDirection.x, cachedPlayerDirection.y, 0f);
        rollElapsedTime = 0f;
        while (rollElapsedTime < _rollLengthTime)
        {
            float rollMovement = Mathf.Lerp(_rollStartVelocity, _rollStartVelocity * 0.75f, rollElapsedTime / _rollLengthTime);
            transform.position += rollDirection * rollMovement * Time.deltaTime;
            rollElapsedTime += Time.deltaTime;
            yield return null;
        }

        IsRolling = false;
        yield return new WaitForSeconds(_rollCoolTime - _rollLengthTime);
        ResetRoll();
    }

    public void OnRollSuccess()
    {
        if (!isRollSuccessFlag)
        {
            isRollSuccessFlag = true;
        }
    }
    #endregion

    #region ATTACKING

    private float attackDelay = 0.5f;

    private bool attackToConsume = false;   // 공격 입력이 들어왔는가?
    private int isAttacking = 0;            // 몇번째 공격중인가?

    private bool canAttack = true;          // 외적으로 공격이 가능한가?
    private bool canAttackFlag = false;     // 쿨타임적으로 공격이 가능한지?

    private void HandleAttacking()
    {
        if (!attackToConsume) return;

        if (IsRolling) return;
        
        if (canAttack && canAttackFlag)
        {
            var attackDirection = new Vector2(_frameInput.MousePosition.x - transform.position.x, _frameInput.MousePosition.y - transform.position.y).normalized;
            Attacked?.Invoke(attackDirection);
        }

        attackToConsume = false;
    }

    public void ChangePlayerStateAttack()
    {

    }

    #endregion
}

public interface IPlayerController
{
    // Rolling, Direction
    public event Action<bool, Vector2> RollingChanged;

    // Direction
    public event Action<Vector2> Attacked;
    public event Action AttackEnd;

    // IsStance
    // haveBullet, Direction
    public event Action<bool> ShootStanceChanged;
    public event Action<bool, Vector2> Shotted;

    public Vector2 MousePosition { get; set; }
    public Vector2 PlayerInput { get; }
    public Vector2 PlayerDirection { get; }

    public bool IsRolling { get; }

    public void OnRollSuccess();
    public void ChangePlayerStateAttack();
}