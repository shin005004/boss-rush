using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : BossController, ISlimeController
{
    #region EXTERNAL
    public Transform SlimeVisualTransform;
    public SlimeAction CurrentAction => currentActionId;
    public event Action<SlimeAction, float> ChangeSlimeAction;
    public event Action<int> BigJumpAnimationEvent;
    public event Action<int> StompAnimationEvent;
    #endregion

    protected int BossHP;
    protected PlayerController _player;
    protected IPlayerController _playerController;


    protected override void Start()
    {
        base.Start();

        _player = InstanceManager.Instance.PlayerController;
        _playerController = _player.GetComponent<PlayerController>();

        currentActionId = SlimeAction.BigJump;
    }

    protected override void Update()
    {
        base.Update();

        HandleBrain();
        MoveTowardPlayer();
        HandleBigJump();
        HandleStomp();
    }

    #region JUMP_ATTACK

    private bool isJumping = false;
    private float maxJumpDistance = 4f;
    private Vector2 jumpStartPosition;
    private Vector2 jumpDestination;
    private void HandleBigJump()
    {
        if (nextActionId != SlimeAction.BigJump) return;
        if (isJumping) return;
        nextActionId = SlimeAction.Idle;

        isJumping = true;

        jumpStartPosition = transform.position;

        var direction = _player.transform.position - transform.position;
        if (direction.magnitude > maxJumpDistance)
            direction = direction.normalized * maxJumpDistance;

        jumpDestination = direction;
        if (jumpDestination.x == 0f)
            jumpDestination.x = 0.01f;

        jumpModifierB = (jumpDestination.y / jumpModifierA - Mathf.Pow(jumpDestination.x, 2f)) / jumpDestination.x;

        ChangeSlimeAction?.Invoke(SlimeAction.BigJump, 2f + jumpDestination.magnitude * 0.2f);
        StartCoroutine(BigJump());
    }

    // y = a(x^2 + bx)
    private float jumpModifierA = -1.2f;
    private float jumpModifierB = 0f;

    private IEnumerator BigJump()
    {
        BigJumpAnimationEvent?.Invoke(1);
        yield return new WaitForSeconds(1f);

        BigJumpAnimationEvent?.Invoke(2);
        float totalTime = jumpDestination.magnitude * 0.2f;
        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetPosition = transform.position;
            float tempX = jumpDestination.x / totalTime * elapsedTime;

            targetPosition.x = jumpStartPosition.x + tempX;
            targetPosition.y = jumpStartPosition.y + jumpModifierA * (Mathf.Pow(tempX, 2f)
                + jumpModifierB * tempX);
            transform.position = targetPosition;

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        isJumping = false;
        chooseNextAction = true;
    }

    #endregion

    #region STOMP_ATTACK

    private void HandleStomp()
    {
        if (nextActionId != SlimeAction.Stomp) return;
        if (isJumping) return;
        nextActionId = SlimeAction.Idle;

        isJumping = true;

        jumpStartPosition = transform.position;
        jumpDestination = _player.transform.position;

        ChangeSlimeAction?.Invoke(SlimeAction.Stomp, 10f);
        StartCoroutine(StompAttack());
    }

    private IEnumerator StompAttack()
    {
        StompAnimationEvent?.Invoke(1);
        yield return new WaitForSeconds(1f);

        StompAnimationEvent?.Invoke(2);
        float totalTime = 2f;
        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetPosition = transform.position;

            targetPosition.y += 50f / totalTime * Time.deltaTime;
            transform.position = targetPosition;

            yield return null;
        }

        Vector3 destination = jumpDestination;
        destination.y += 50f;
        transform.position = destination;

        StompAnimationEvent?.Invoke(3);
        totalTime = 1f;
        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetPosition = transform.position;

            targetPosition.y -= 50f / totalTime * Time.deltaTime;
            transform.position = targetPosition;

            yield return null;
        }

        StompAnimationEvent?.Invoke(4);
        yield return new WaitForSeconds(3f);

        isJumping = false;
        chooseNextAction = true;
    }

    #endregion

    #region MOVE
    [SerializeField] private float bossSpeed = 10f;
    private void MoveTowardPlayer()
    {
        if (nextActionId == SlimeAction.Move)
        {
            nextActionId = SlimeAction.Idle;
            StartCoroutine(MoveReset());
        }
        if (currentActionId != SlimeAction.Move) return;

        var destination = _player.transform.position;
        var direction = (destination - transform.position).normalized;

        HandleBossFlipping(direction);
        transform.position += direction * bossSpeed * Time.deltaTime;
    }

    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 flippedScale = new Vector3(-1f, 1f, 1f);
    private void HandleBossFlipping(Vector3 playerDirection)
    {
        if (playerDirection.x < 0)
            SlimeVisualTransform.localScale = flippedScale;
        else
            SlimeVisualTransform.localScale = normalScale;
    }

    private IEnumerator MoveReset()
    {
        yield return new WaitForSeconds(3f);

        chooseNextAction = true;
    }
    #endregion

    #region BRAIN
    private bool chooseNextAction = true;
    private SlimeAction nextActionId = 0;
    private SlimeAction currentActionId = 0;
    //private float currentActionTime = 0f;

    private void HandleBrain()
    {
        if (!chooseNextAction) return;

        chooseNextAction = false;

        switch (currentActionId)
        {
            case SlimeAction.Idle:
        
                currentActionId = SlimeAction.Move;
                nextActionId = SlimeAction.Move;

                break;

            case SlimeAction.Move:

                currentActionId = SlimeAction.Stomp;
                nextActionId = SlimeAction.Stomp;

                break;

            case SlimeAction.BigJump:

                currentActionId = SlimeAction.Move;
                nextActionId = SlimeAction.Move;

                break;

            case SlimeAction.Stomp:

                currentActionId = SlimeAction.Move;
                nextActionId = SlimeAction.Move;

                break;
        }

    }

    #endregion
}

public enum SlimeAction
{
    Idle = 0,
    Move = 1,
    Attack = 2,
    BigJump = 3,
    Stomp = 4,
}

public interface ISlimeController
{
    public SlimeAction CurrentAction { get; }
    public event Action<SlimeAction, float> ChangeSlimeAction;
    public event Action<int> BigJumpAnimationEvent;
    public event Action<int> StompAnimationEvent;
}

public class BossStatsSO
{
    public int BossMaxHP;
}