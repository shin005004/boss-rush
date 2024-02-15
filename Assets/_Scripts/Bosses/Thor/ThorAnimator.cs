using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThorAnimator : MonoBehaviour
{
    private IThorController _thorController;
    [SerializeField] private Animator _anim;

    private void Awake()
    {
        _thorController = GetComponent<IThorController>();
    }

    private void Start()
    {
        _thorController.ChangeThorAction += OnThorActionChanged;
        _thorController.AnimationEvent += AnimationChanged;
    }

    private void Update()
    {
        HandleAnimations();
    }

    private ThorAction currentAction;
    private float currentAnimationTime;
    private void OnThorActionChanged(ThorAction thorAction, float animationTime)
    {
        currentAction = thorAction;
        currentAnimationTime = animationTime;
    }

    private void AnimationChanged(int attackId, int animationNumber)
    {
        switch(attackId)
        {
            case 1:
                _anim.SetTrigger("Throw");
                break;
            case 0:
                _anim.SetTrigger("ThrowBack");
                break;
        }
    }

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
            //if (currentAction == ThorAction.Idle)
            //    return LockState(Idle, currentAnimationTime);

            //// Stomp
            if (currentAction == ThorAction.ThrowAttack)
                return LockState(Attack, currentAnimationTime);

            if (currentAction == ThorAction.ChantAttack)
                return LockState(Chant, currentAnimationTime);

            if (currentAction == ThorAction.FloorAttack)
                return LockState(Floor, currentAnimationTime);

            if (currentAction == ThorAction.Move)
                return LockState(Move, currentAnimationTime);



            return currentState;

            int LockState(int s, float t)
            {
                lockedTill = Time.time + t;
                return s;
            }
        }

        void ResetFlags()
        {
            currentAction = ThorAction.Idle;
        }
    }
    #endregion

    #region HASHING

    private static readonly int Idle = Animator.StringToHash("Idle");
    private static readonly int Move = Animator.StringToHash("Move");

    private static readonly int Attack = Animator.StringToHash("Attack");
    private static readonly int Chant = Animator.StringToHash("Chant");
    private static readonly int Floor = Animator.StringToHash("Floor");

    #endregion
}
