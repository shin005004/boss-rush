using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    [Header("Animation")]
    public Animator WeaponAnimator;

    [Header("VFX")]
    public Vector3 SpawnOffset;
    public GameObject SwordSlashVFX;

    public void SpawnVFX()
    {
        Instantiate(SwordSlashVFX, transform.position + SpawnOffset, Quaternion.identity);
    }
}
