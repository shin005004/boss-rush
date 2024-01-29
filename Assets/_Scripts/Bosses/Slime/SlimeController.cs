using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlimeController : BossController, ISlimeController
{
    #region EXTERNAL
    public Transform SlimeVisualTransform;
    public SlimeAction CurrentAction => currentActionId;
    public event Action<SlimeAction, float> ChangeSlimeAction;
    public event Action<int> BigJumpAnimationEvent;
    public event Action<int> StompAnimationEvent;
    public event Action<int> ShockwaveAnimationEvent;

    [SerializeField] private Collider2D bossCollider;
    #endregion

    #region VFX
    [SerializeField] private GameObject WindShockwave;
    [SerializeField] private GameObject DustShockwave;
    [SerializeField] private GameObject FloorCrater;
    [SerializeField] private GameObject GroundShockwave;
    #endregion

    protected int BossHP = 100;
    protected PlayerController _player;
    protected IPlayerController _playerController;

    private SlimeWarner _slimeWarner;

    protected override void Start()
    {
        base.Start();

        _player = InstanceManager.Instance.PlayerController;
        _playerController = _player.GetComponent<PlayerController>();

        _slimeWarner = GetComponent<SlimeWarner>();

        currentActionId = SlimeAction.BigJump;
    }

    protected override void Update()
    {
        base.Update();

        HandleBrain();
        MoveTowardPlayer();
        HandleBigJump();
        HandleStomp();
        HandleShockwave();
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

        if (direction.x == 0f)
            direction.x = 0.01f;

        jumpDestination = jumpStartPosition + (Vector2)direction;
        jumpModifierA = -1.2f;
        jumpModifierB = (direction.y / jumpModifierA - Mathf.Pow(direction.x, 2f)) / direction.x;

        ChangeSlimeAction?.Invoke(SlimeAction.BigJump, 2f + direction.magnitude * 0.2f);
        StartCoroutine(BigJump());
    }

    // y = a(x^2 + bx)
    private float jumpModifierA = -1.2f;
    private float jumpModifierB = 0f;

    private IEnumerator BigJump()
    {
        var direction = jumpDestination - jumpStartPosition;

        BigJumpAnimationEvent?.Invoke(1);
        _slimeWarner.ShowBigjumpWarner(jumpDestination, true);

        yield return new WaitForSeconds(1f);

        bossCollider.enabled = false;

        BigJumpAnimationEvent?.Invoke(2);
        float totalTime = direction.magnitude * 0.2f;
        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetPosition = transform.position;
            float tempX = direction.x / totalTime * elapsedTime;

            targetPosition.x = jumpStartPosition.x + tempX;
            targetPosition.y = jumpStartPosition.y + jumpModifierA * (Mathf.Pow(tempX, 2f)
                + jumpModifierB * tempX) + 1.25f - (Mathf.Abs(elapsedTime - totalTime / 2f)) *
                (Mathf.Abs(elapsedTime - totalTime / 2f)) * 5f;
            transform.position = targetPosition;

            yield return null;
        }

        bossCollider.enabled = true;

        _slimeWarner.ShowBigjumpWarner(jumpDestination, false);
        Instantiate(WindShockwave, jumpDestination, Quaternion.identity);

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

        _slimeWarner.ShowStompWarner(jumpDestination + new Vector2(0f, -1.4f), true);
        yield return new WaitForSeconds(1f);

        bossCollider.enabled = false;

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
            Vector2 targetOffset = Vector2.zero;

            targetOffset.y = 50f - elapsedTime * 50f;
            transform.position = jumpDestination + targetOffset;

            yield return null;
        }

        bossCollider.enabled = true;

        _slimeWarner.ShowStompWarner(jumpDestination + new Vector2(0f, -1.4f), false);
        Instantiate(DustShockwave, jumpDestination + new Vector2(0f, -1.4f), Quaternion.identity);

        StompAnimationEvent?.Invoke(4);
        yield return new WaitForSeconds(3f);

        isJumping = false;
        chooseNextAction = true;
    }

    #endregion

    #region GROUNDSHOCKWAVE
    private void HandleShockwave()
    {
        if (nextActionId != SlimeAction.GroundShockwave) return;
        if (isJumping) return;
        nextActionId = SlimeAction.Idle;

        isJumping = true;

        jumpStartPosition = transform.position;
        jumpDestination = Vector3.zero;

        var direction = jumpDestination - jumpStartPosition;

        if (direction.x == 0f)
            direction.x = 0.01f;

        jumpModifierA = -3f;
        jumpModifierB = (direction.y / jumpModifierA - Mathf.Pow(direction.x, 2f)) / direction.x;

        // Debug.Log("HI");
        ChangeSlimeAction?.Invoke(SlimeAction.GroundShockwave, 10f);
        StartCoroutine(GroundSHockwave());
    }

    private IEnumerator GroundSHockwave()
    {
        var direction = jumpDestination - jumpStartPosition;

        ShockwaveAnimationEvent?.Invoke(1);
        _slimeWarner.ShowShockwaveWarner(jumpDestination);

        yield return new WaitForSeconds(2f);

        ShockwaveAnimationEvent?.Invoke(2);

        bossCollider.enabled = false;

        float totalTime = 1f;
        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetPosition = transform.position;
            float tempX = direction.x / totalTime * elapsedTime;

            targetPosition.x = jumpStartPosition.x + tempX;
            targetPosition.y = jumpStartPosition.y + jumpModifierA * (Mathf.Pow(tempX, 2f)
                + jumpModifierB * tempX) + 1.25f - (Mathf.Abs(elapsedTime - totalTime / 2f)) *
                (Mathf.Abs(elapsedTime - totalTime / 2f)) * 5f;
            transform.position = targetPosition;

            yield return null;
        }

        bossCollider.enabled = true;
        Instantiate(FloorCrater, jumpDestination + new Vector2(0f, -0.42f), Quaternion.identity);

        ShockwaveAnimationEvent?.Invoke(3);

        StartCoroutine(SpawnGroundShockwave());
        yield return new WaitForSeconds(1f);

        isJumping = false;
        chooseNextAction = true;
    }

    private IEnumerator SpawnGroundShockwave()
    {
        var totalTime = 5f;
        float objectDistance = 0.7f, lastSpawnTime = 0f;
        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            float radius = 1.8f + elapsedTime * 2;
            if(elapsedTime - lastSpawnTime < 0.3f )
            {
                yield return null;
                continue;
            }

            Vector2 lastObjectPosition = Vector2.zero;
            for (float angle = 0; angle < 360f; angle += 1f)
            {
                var targetPosition = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad) * 0.8f);
                targetPosition *= radius;

                if ((targetPosition - lastObjectPosition).magnitude > objectDistance)
                {
                    Instantiate(GroundShockwave, targetPosition, Quaternion.identity);
                    lastObjectPosition = targetPosition;
                }
            }

            lastSpawnTime = elapsedTime;
            yield return null;
        }
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
        yield return new WaitForSeconds(5f);

        chooseNextAction = true;
    }
    #endregion

    #region BRAIN
    private bool chooseNextAction = true;
    private SlimeAction nextActionId = 0;
    private SlimeAction currentActionId = 0;
    //private float currentActionTime = 0f;

    private int actionCount = 0;

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

                if (actionCount < 3)
                {
                    int action = UnityEngine.Random.Range(0, 2);
                    if (action == 0)
                    {
                        currentActionId = nextActionId = SlimeAction.BigJump;
                    }
                    else
                        currentActionId = nextActionId = SlimeAction.Stomp;

                    actionCount++;
                }
                else
                {
                    actionCount = 0;
                    currentActionId = nextActionId = SlimeAction.GroundShockwave;
                }

                break;

            case SlimeAction.BigJump:

                currentActionId = SlimeAction.Move;
                nextActionId = SlimeAction.Move;

                break;

            case SlimeAction.Stomp:

                currentActionId = SlimeAction.Move;
                nextActionId = SlimeAction.Move;

                break;

            case SlimeAction.GroundShockwave:

                currentActionId = SlimeAction.Move;
                nextActionId = SlimeAction.Move;

                break;
        }

    }

    #endregion

    #region HIT

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.transform.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            _playerController.OnBossHit();
            if (currentActionId == SlimeAction.Move)
                StartCoroutine(HitKnockback());

            OnBossHit();
            BossHP--;

            if (BossHP == 0)
                Debug.Log("BossDeath");
            Debug.Log("BossHit");
        }
    }

    [SerializeField] private BossSceneUI bossSceneUI;

    private void OnBossHit()
    {
        bossSceneUI.Blood -= 2;
        GameManager.Instance.BloodManager.AddBlood(5);
    }



    private float knockbackStartVelocity = 6f;

    private float knockbackElapsedTime = 0f;
    private float knockbackLengthTime = 0.1f;

    private IEnumerator HitKnockback()
    {
        Vector3 knockbackDirection = (transform.position - _player.transform.position).normalized;

        knockbackElapsedTime = 0f;
        while (knockbackElapsedTime < knockbackLengthTime)
        {
            float rollMovement = Mathf.Lerp(knockbackStartVelocity, knockbackStartVelocity * 0.2f, knockbackElapsedTime / knockbackLengthTime);
            transform.position += knockbackDirection * rollMovement * Time.deltaTime;
            knockbackElapsedTime += Time.deltaTime;
            yield return null;
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
    GroundShockwave = 5,
}

public interface ISlimeController
{
    public SlimeAction CurrentAction { get; }
    public event Action<SlimeAction, float> ChangeSlimeAction;
    public event Action<int> BigJumpAnimationEvent;
    public event Action<int> StompAnimationEvent;
    public event Action<int> ShockwaveAnimationEvent;
}

public class BossStatsSO
{
    public int BossMaxHP;
}