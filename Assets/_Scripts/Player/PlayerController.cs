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

    public event Action<bool, Vector2> DashingChanged;

    public event Action<Vector2> Attacked;
    public event Action AttackEnd;

    public event Action<bool> ShootStanceChanged;
    public event Action<bool, Vector2> Shotted;

    public Vector2 MousePosition { get; set; }
    public Vector2 PlayerInput => currentPlayerDirection;
    public Vector2 PlayerDirection => cachedPlayerDirection;

    public bool IsDashing { get; private set; }
    #endregion

    private void Start()
    {
        _input = GetComponent<PlayerInput>();

        // Setup
        _playerSpeed = PlayerStats.PlayerSpeed;
        _dashMovementModifier = PlayerStats.DashMovementModifier;

        _dashStartVelocity = PlayerStats.DashStartVelocity;
        _dashLengthTime = PlayerStats.DashLengthTime;
        _dashCoolTime = PlayerStats.DashCoolTime;
        _dashStartUp = PlayerStats.DashStartUp;
        _dashInvlunTime = PlayerStats.DashInvlunTime;

        _justDashPrefab = PlayerStats.JustDashPrefab;
    }

    #region UPDATE
    private void Update()
    {
        GatherInput();
        HandleMoving();
        HandleDashing();
    }
    #endregion


    #region INPUT
    private void GatherInput()
    {
        _frameInput = _input.FrameInput;
        MousePosition = (Vector2)_frameInput.MousePosition;

        if (_frameInput.DashDown && !IsDashing) dashToConsume = true;
    }
    #endregion

    #region MOVING
    private float _playerSpeed;
    private float _dashMovementModifier;

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

        var posX = transform.position.x + (_playerSpeed / _dashMovementModifier) * currentPlayerDirection.x * Time.deltaTime;
        var posY = transform.position.y + (_playerSpeed / _dashMovementModifier) * currentPlayerDirection.y * Time.deltaTime;
        transform.position = new Vector3(posX, posY, 0f);
    }
    #endregion

    #region DASHING
    private float _dashStartVelocity;   // 구르기 시작 속도
    private float _dashLengthTime;      // 전체 구르기 시간
    private float _dashCoolTime;        // 구르기 시작부터 쿨다운

    private float _dashStartUp;
    private float _dashInvlunTime;

    private bool canDash = true;            // 외적으로 구르기 가능한지?
    private bool dashToConsume = false;     // 대쉬 해야하는가?
    private bool dashCoolDownFlag = true;   // 쿨다운으로 구르기 가능한지?

    // 구르기 시간 확인하는 시간
    private float dashElapsedTime = 0f;

    // 저스트 구르기 확인용
    private GameObject _justDashPrefab;
    private bool isDashSuccessFlag = false;

    private void HandleDashing()
    {
        if (!dashToConsume) return;

        if (canDash && canMove && dashCoolDownFlag)
        {
            dashCoolDownFlag &= false;
            DashingChanged?.Invoke(true, cachedPlayerDirection);
            StartCoroutine(Dash());
        }

        dashToConsume = false;
    }

    private void ResetDash()
    {

        dashCoolDownFlag = true;
    }

    private IEnumerator Dash()
    {
        GameObject dashDummyObject = Instantiate(_justDashPrefab, transform.position, Quaternion.identity);
        JustDashDummy dashDummyComponent = dashDummyObject.GetComponent<JustDashDummy>();
        dashDummyComponent.StartUp(_dashStartUp, _dashInvlunTime, this);

        Vector3 dashDirection = new Vector3(cachedPlayerDirection.x, cachedPlayerDirection.y, 0f);
        dashElapsedTime = 0f;
        while (dashElapsedTime < _dashLengthTime)
        {
            float dashMovement = Mathf.Lerp(_dashStartVelocity, _dashStartVelocity * 0.75f, dashElapsedTime / _dashLengthTime);
            transform.position += dashDirection * dashMovement * Time.deltaTime;
            dashElapsedTime += Time.deltaTime;
            yield return null;
        }

        IsDashing = false;
        yield return new WaitForSeconds(_dashCoolTime - _dashLengthTime);
        ResetDash();
    }

    public void OnDashSuccess()
    {
        if (!isDashSuccessFlag)
        {
            isDashSuccessFlag = true;
        }
    }
    #endregion
}

public interface IPlayerController
{
    // Dashing, Direction
    public event Action<bool, Vector2> DashingChanged;

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

    public bool IsDashing { get; }

    public void OnDashSuccess();
}