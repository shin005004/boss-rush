using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class MeleeStatsSO : ScriptableObject
{
    [Header("MELEE")]
    public int MeleeID = 0;

    public int MaxWeaponHits = 2;
    public float[] MeleeCoolTime;
    public GameObject WeaponPrefab;
}