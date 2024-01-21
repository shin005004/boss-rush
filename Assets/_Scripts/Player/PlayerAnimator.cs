using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerAnimator : MonoBehaviour
{
    private IPlayerController _playerController;
    [SerializeField] private Animator _anim;


    private void Awake()
    {
        _playerController = GetComponent<IPlayerController>();
        
    }

    private void Start()
    {
        _playerController.RollingChanged += OnRollingChanged;
    }

    #region ROLLING

    private bool isRolling = false;
    private float rollingDirectionDegree;
    private float rollingAnimationTime = 9/16f;

    private void OnRollingChanged(bool isRolling, Vector2 rollDirection)
    {
        this.isRolling = isRolling;
        rollingDirectionDegree = CalculateDegreeWithVector(rollDirection);
    }

    #endregion

    #region MOVING

    private float playerDirectionDegree;

    private void Update()
    {
        playerDirectionDegree = CalculateDegreeWithVector(_playerController.PlayerDirection);
        HandleAnimations();
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

            // Dash
            if (isRolling)
            {
                if (playerDirectionDegree < 90f || playerDirectionDegree > 360f - 90f)
                    return LockState(DodgeForward, rollingAnimationTime);
                return LockState(DodgeBackward, rollingAnimationTime);
            }

            // Idle and Run
            if (playerDirectionDegree < 90f || playerDirectionDegree > 360f - 90f)
                return _playerController.PlayerInput.magnitude <= 0.1f ? IdleBackward : RunBackward;
            return _playerController.PlayerInput.magnitude <= 0.1f ? IdleForward : RunForward;

            int LockState(int s, float t)
            {
                lockedTill = Time.time + t;
                return s;
            }
        }

        void ResetFlags()
        {
            isRolling = false;
        }
    }

    #endregion

    #region HASHING

    private static readonly int IdleForward = Animator.StringToHash("IdleForward");
    private static readonly int IdleBackward = Animator.StringToHash("IdleBackward");

    private static readonly int RunForward = Animator.StringToHash("RunForward");
    private static readonly int RunBackward = Animator.StringToHash("RunBackward");

    private static readonly int DodgeForward = Animator.StringToHash("DodgeForward");
    private static readonly int DodgeBackward = Animator.StringToHash("DodgeBackward");

    #endregion

    #region Utils
    // Upward Is 0
    // Clockwise
    private float CalculateDegreeWithVector(Vector2 lookDirection)
        => (270f + Mathf.Atan2(lookDirection.y, lookDirection.x) / Mathf.PI * 180f) % 360f;

    private AnimatorClipInfo[] clipInfo;
    public string GetCurrentClipName(Animator animator)
    {
        int layerIndex = 0;
        clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
    #endregion
}
