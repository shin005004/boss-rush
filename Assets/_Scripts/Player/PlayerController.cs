using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerController : MonoBehaviour, IPlayerController
{
    #region INTERNAL
    private PlayerInput _input;
    private FrameInput _frameInput;

    private int _playerHP;

    private bool _hasControl;
    #endregion

    #region EXTERNAL
    public Transform PlayerVisualTransform;
    public PlayerStatSO PlayerStats;

    public event Action<bool, Vector2> RollingChanged;

    public event Action<Vector2> Attacked;
    // public event Action AttackEnd;

    //public event Action<bool> ShootStanceChanged;
    //public event Action<bool, Vector2> Shotted;

    public Vector2 MousePosition { get; set; }
    public Vector2 PlayerInput => currentPlayerDirection;
    public Vector2 PlayerDirection => cachedPlayerDirection;
    public PlayerStatSO Stats => PlayerStats;

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
        _rollInvulnTime = PlayerStats.RollInvulnTime;

        _justRollPrefab = PlayerStats.JustRollPrefab;
    }
    #endregion

    #region UPDATE
    private void FixedUpdate()
    {
        GatherInput();
        HandlePlayerFlipping();

        HandleMoving();
        HandleRolling();
        HandleAttacking();
    }
    #endregion

    #region INPUT
    private void GatherInput()
    {
        _frameInput = _input.FrameInput;
        MousePosition = (Vector2)_frameInput.MousePosition;

        if (_frameInput.RollDown && !IsRolling) rollToConsume = true;
        if (_frameInput.AttackDown && !IsRolling) attackToConsume = true;
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
        if (!canMove) return;

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
                PlayerVisualTransform.localScale = flippedScale;
            else
                PlayerVisualTransform.localScale = normalScale;
        }
    }
    #endregion

    #region ROLLING
    private float _rollStartVelocity;   // 구르기 시작 속도
    private float _rollLengthTime;      // 전체 구르기 시간
    private float _rollCoolTime;        // 구르기 시작부터 쿨다운

    private float _rollStartUp;
    private float _rollInvulnTime;

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
        isRollSuccessFlag = false;
        rollCoolDownFlag = true;
    }

    private IEnumerator Roll()
    {
        IsRolling = true;

        GameObject rollDummyObject = Instantiate(_justRollPrefab, transform.position, Quaternion.identity);
        JustRollDummy rollDummyComponent = rollDummyObject.GetComponent<JustRollDummy>();
        rollDummyComponent.StartUp(_rollStartUp, _rollInvulnTime, this);

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
            Debug.Log("RollSuccess");
            isRollSuccessFlag = true;
        }
    }
    #endregion

    #region ATTACKING

    // private float attackDelay = 0.5f;

    private bool attackToConsume = false;   // 공격 입력이 들어왔는가?
    // private int isAttacking = 0;            // 몇번째 공격중인가?

    private bool canAttack = true;          // 외적으로 공격이 가능한가?
    private bool canAttackFlag = true;     // 쿨타임적으로 공격이 가능한지?

    private Vector2 attackDirection;

    private void HandleAttacking()
    {
        if (!attackToConsume) return;

        if (IsRolling) return;
        
        if (canAttack && canAttackFlag)
        {
            attackDirection = new Vector2(_frameInput.MousePosition.x - transform.position.x, _frameInput.MousePosition.y - transform.position.y).normalized;
            Attacked?.Invoke(attackDirection);
        }

        attackToConsume = false;
    }

    public void ChangePlayerStateAttack(bool canMove, bool canRoll)
    {
        this.canMove = canMove;
        this.canRoll = canRoll;
    }

    #endregion

    #region HIT
    private int playerHP = 3;
    private float lastHitTime = 0f;
    private float hitInvulnTime = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log(collision.transform.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("BossHitbox"))
        {
            if (Time.time - lastHitTime < hitInvulnTime)
                return;

            if (IsRolling && rollElapsedTime > _rollStartUp && rollElapsedTime < _rollInvulnTime)
                OnRollSuccess();
            else
            {
                playerHP -= 1;
                Debug.Log("PlayerHit");

                lastHitTime = Time.time;

                if(playerHP == 0)
                    Debug.Log("Death");
            }
        }
    }

    private float knockbackStartVelocity = 8f;

    private float knockbackElapsedTime = 0f;
    private float knockbackLengthTime = 0.1f;

    public void OnBossHit()
    {
        StartCoroutine(HitKnockback());
    }

    private IEnumerator HitKnockback()
    {
        Vector3 knockbackDirection = -attackDirection;

        knockbackElapsedTime = 0f;
        while (knockbackElapsedTime < knockbackLengthTime)
        {
            float rollMovement = Mathf.Lerp(knockbackStartVelocity, knockbackStartVelocity * 0.2f, knockbackElapsedTime / knockbackLengthTime);
            transform.position += knockbackDirection * rollMovement * Time.deltaTime;
            knockbackElapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    public void OnWeaponSwing()
    {
        StartCoroutine(WeaponSwing());
    }

    private float forceLengthTime = 0.25f;
    private float forceStartVelocity = 12f;
    private IEnumerator WeaponSwing()
    {
        Vector3 forceDirection = attackDirection;

        float forceElapsedTime = 0f;
        while (forceElapsedTime < forceLengthTime)
        {
            float rollMovement = Mathf.Lerp(forceStartVelocity, forceStartVelocity * 0.2f, forceLengthTime / forceElapsedTime);
            transform.position += forceDirection * rollMovement * Time.deltaTime;
            forceElapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    #endregion
}

public interface IPlayerController
{
    // Rolling, Direction
    public event Action<bool, Vector2> RollingChanged;

    // Direction
    public event Action<Vector2> Attacked;

    // IsStance
    // haveBullet, Direction
    //public event Action<bool> ShootStanceChanged;
    //public event Action<bool, Vector2> Shotted;

    public Vector2 MousePosition { get; set; }
    public Vector2 PlayerInput { get; }
    public Vector2 PlayerDirection { get; }
    public PlayerStatSO Stats { get; }

    public bool IsRolling { get; }

    public void OnRollSuccess();
    public void ChangePlayerStateAttack(bool canMove, bool canRoll);
    public void OnBossHit();
    public void OnWeaponSwing();
}