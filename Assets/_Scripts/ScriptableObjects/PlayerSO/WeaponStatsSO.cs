using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class MeleeStatsSO : ScriptableObject
{
    [Header("MELEE")]
    public int MeleeID = 0;

    public float MeleeCoolTime = 0.5f;
}

[CreateAssetMenu]
public class RangedStatsSO : ScriptableObject
{
    [Header("RANGED")]
    public int RangedID = 0;

    public int MaxAmmo = 4;
    public int RequiredHits = 1;

    public bool Stance = false;

    public float ShootChargeTime = 0.75f;
    public float ShootCoolTime = 0.15f;
}