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
    }

    #region UPDATE

    private void Update()
    {
        HandleAnimations();
    }

    #endregion

    #region BRAIN
    
    private void OnSlimeActionChanged(SlimeAction slimeAction, float animationTime)
    {
        switch (slimeAction)
        {
            case SlimeAction.BigJump:
                isJumping = true;
                bigJumpAnimationTime = animationTime;
                break;
        }
    }

    #endregion

    #region JUMP

    private bool isJumping = false;
    private float bigJumpAnimationTime = 1f;

    private void BigJumpAnimationChanged(int animationNumber)
    {
        _anim.SetTrigger("BigJump");
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
            if (isJumping)
                return LockState(Jump, bigJumpAnimationTime);

            // Idle and Run
            if (_slimeController.CurrentAction == SlimeAction.Move)
                return Move;

            return Idle;

            int LockState(int s, float t)
            {
                lockedTill = Time.time + t;
                return s;
            }
        }

        void ResetFlags()
        {
            isJumping = false;
        }
    }

    #endregion

    #region HASHING

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");

    private static readonly int Jump = Animator.StringToHash("Jump");

    #endregion
}
