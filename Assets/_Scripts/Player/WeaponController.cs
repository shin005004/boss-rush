using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private IPlayerController _playerController;

    [SerializeField] private Transform _weaponTransform;
    [SerializeField] private GameObject _weaponSlashEffect;

    private bool _isShooting;

    private void Awake()
    {

    }

    private void Update()
    {
        Vector2 mouseDirection = (_playerController.MousePosition - (Vector2)transform.position).normalized;
        _weaponTransform.right = mouseDirection;


    }

    #region ATTAKCING

    private MeleeStatsSO _meleeStatsSO;

    private int isAttacking = 0;
    private bool attackToConsume = false;
    private float attackAngle;

    private void OnAttacked(Vector2 attackDirection)
    {
        if (isAttacking == 0)
        {

        }

        attackAngle = CalculateAngle(attackDirection, _playerController.PlayerInput);

        var spawnAngle = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        Quaternion effectRotation = Quaternion.AngleAxis(spawnAngle, Vector3.forward);
        GameObject weaponEffect = Instantiate(_weaponSlashEffect, transform.position, effectRotation);
    }

    public static float CalculateAngle(Vector3 from, Vector3 to) => Quaternion.FromToRotation(Vector3.up, to - from).eulerAngles.z;


    #endregion

    #region UTILS
    private AnimatorClipInfo[] clipInfo;
    public string GetCurrentClipName(Animator animator)
    {
        int layerIndex = 0;
        clipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex);
        return clipInfo[0].clip.name;
    }
    #endregion
}
