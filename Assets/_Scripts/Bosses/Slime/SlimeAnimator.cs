using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeAnimator : MonoBehaviour
{
    private ISlimeController _slimeController;
    [SerializeField] private Animator _anim;

    private void Awake()
    {
        _slimeController = GetComponent<ISlimeController>();
    }

    private void Start()
    {
        _slimeController.ChangeSlimeAction += OnSlimeActionChanged;
        _slimeController.BigJumpAnimationEvent += BigJumpAnimationChanged;
        _slimeController.StompAnimationEvent += StompAnimationChanged;
    }

    #region UPDATE

    private void Update()
    {
        HandleAnimations();
    }

    #endregion

    #region BRAIN
    private SlimeAction currentAction;
    private void OnSlimeActionChanged(SlimeAction slimeAction, float animationTime)
    {
        currentAction = slimeAction;

        switch (slimeAction)
        {
            case SlimeAction.BigJump:        
                bigJumpAnimationTime = animationTime;
                break;
            case SlimeAction.Stomp:
                stompAnimationTime = animationTime;
                break;
        }
    }

    #endregion

    #region JUMP

    
    private float bigJumpAnimationTime = 0.5f;

    private void BigJumpAnimationChanged(int animationNumber)
    {
        _anim.SetTrigger("BigJump");
    }

    #endregion


    #region STOMP

    private float stompAnimationTime = 1f;

    private void StompAnimationChanged(int animationNumber)
    {
        _anim.SetTrigger("Stomp");
    }
    #endregion

    #region ANIMATION
    private float lockedTill;
    private int currentState;

    private void HandleAnimations()
    {
        var state = GetState();
        ResetFlags();

        if (state == currentState) return;

        _anim.Play(state, 0);
        currentState = state;

        int GetState()
        {
            if (Time.time < lockedTill) return currentState;

            // BigJump
            if (currentAction == SlimeAction.BigJump)
                return LockState(Jump, bigJumpAnimationTime);

            // Stomp
            if (currentAction == SlimeAction.Stomp)
                return LockState(Jump, stompAnimationTime);


            // Idle and Run
            if (_slimeController.CurrentAction == SlimeAction.Move)
                return Move;

            return currentState;

            int LockState(int s, float t)
            {
                lockedTill = Time.time + t;
                return s;
            }
        }

        void ResetFlags()
        {
            currentAction = SlimeAction.Idle;
        }
    }

    #endregion

    #region HASHING

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");

    private static readonly int Jump = Animator.StringToHash("Jump");

    #endregion
}
