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

    public void SpawnVFX(int vfxID)
    {
        Vector3 spawnOffset;
        GameObject vfx;

        switch (vfxID)
        {
            case 1:
                spawnOffset = transform.right * SpawnOffset.x * transform.parent.localScale.x + transform.up * SpawnOffset.y;
                vfx = Instantiate(SwordSlashVFX, transform.position + spawnOffset, transform.rotation);
                vfx.transform.localScale = transform.parent.localScale * 0.9f;
                break;

            case 2:
                spawnOffset = transform.right * SpawnOffset.x * transform.parent.localScale.x + transform.up * SpawnOffset.y;
                vfx = Instantiate(SwordSlashVFX, transform.position + spawnOffset, transform.rotation);
                vfx.transform.localScale = new Vector3(transform.parent.localScale.x, -1f, 1f) * 0.9f;
                break;
        }
        
    }
}
