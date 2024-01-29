using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    private IPlayerController _playerController;
    private Sword _swordWeapon;

    [SerializeField] private Transform _weaponTransform;
    //[SerializeField] private GameObject _weaponSlashEffect;

    private void Awake()
    {
        _playerController = GetComponent<IPlayerController>();
        _playerController.Attacked += OnAttacked;
    }

    private void Start()
    {
        _meleeStatsSO = _playerController.Stats.MeleeStats;
        var weaponGO = Instantiate(_meleeStatsSO.WeaponPrefab, _weaponTransform);
        _swordWeapon = weaponGO.GetComponent<Sword>();
    }

    private void Update()
    {
        HandleTransformFlipping();
        HandleAttacking();
    }

    private Vector3 normalScale = new Vector3(1f, 1f, 1f);
    private Vector3 flippedScale = new Vector3(-1f, 1f, 1f);
    private void HandleTransformFlipping()
    {
        Vector2 mouseDirection = (_playerController.MousePosition - (Vector2)transform.position).normalized;

        if (mouseDirection.x < 0)
        {
            _weaponTransform.localScale = flippedScale;

            float weaponAngle = -Mathf.Atan2(mouseDirection.y, Mathf.Abs(mouseDirection.x)) * Mathf.Rad2Deg;
            Quaternion weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);
            _weaponTransform.rotation = weaponRotation;
        }
        else
        {
            _weaponTransform.localScale = normalScale;

            float weaponAngle = Mathf.Atan2(mouseDirection.y, Mathf.Abs(mouseDirection.x)) * Mathf.Rad2Deg;
            Quaternion weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);
            _weaponTransform.rotation = weaponRotation;
        }
            
    }

    #region ATTAKCING

    private MeleeStatsSO _meleeStatsSO;

    private int isAttacking = 0;                // 몇번째 공격중인가?
    private bool attackPreInputFlag = true;     // 선입력 가능한 시간을 확인
    private bool attackCoolFlag = true;         // 공격 후딜이 모두 지나있는가
    private bool attackToConsume = false;       // 선입력이 걸려있는가

    private Vector2 _cachedAttackDirection;
    private void OnAttacked(Vector2 attackDirection)
    {
        if (!attackPreInputFlag || isAttacking >= _meleeStatsSO.MaxWeaponHits) return;
        attackToConsume = true;
        _cachedAttackDirection = attackDirection;

        string boolName = "Attack" + (isAttacking + 1).ToString();
        _swordWeapon.WeaponAnimator.SetBool(boolName, true);
    }

    private IEnumerator AttackInputDelay(float delayTime)
    {
        string boolName = "Attack" + isAttacking.ToString();

        yield return new WaitForSeconds(delayTime - 0.25f);
        attackPreInputFlag = true;
        _swordWeapon.WeaponAnimator.SetBool(boolName, false);

        yield return new WaitForSeconds(0.3f);
        attackCoolFlag = true;        
    }

    private void HandleAttacking()
    {
        if (isAttacking != 0 && attackToConsume == false && attackCoolFlag == true)
        {
            isAttacking = 0;
            _playerController.ChangePlayerStateAttack(true, true);
        }

        if (!attackToConsume) return;
        if (!attackCoolFlag) return;

        attackToConsume = false;
        attackPreInputFlag = false;
        attackCoolFlag = false;

        isAttacking += 1;

        //string boolName = "Attack" + isAttacking.ToString();
        //_swordWeapon.WeaponAnimator.SetBool(boolName, true);

        _playerController.OnWeaponSwing();
        StartCoroutine(AttackInputDelay(_meleeStatsSO.MeleeCoolTime[isAttacking - 1]));

        if (isAttacking == 1)
        {
            _playerController.ChangePlayerStateAttack(false, false);
        }

        // attackAngle = CalculateAngle(_cachedAttackDirection, transform.root.forward);

        //var spawnAngle = Mathf.Atan2(_cachedAttackDirection.y, _cachedAttackDirection.x) * Mathf.Rad2Deg;
        //Quaternion effectRotation = Quaternion.AngleAxis(spawnAngle, Vector3.forward);
        //GameObject weaponEffect = Instantiate(_weaponSlashEffect, transform.position, effectRotation);
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
