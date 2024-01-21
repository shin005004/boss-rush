using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats")]
public class PlayerStatSO : ScriptableObject
{
    [Header("HEALTH")]
    public int PlayerHP = 3;

    [Header("MOVEMENT")]
    public float PlayerSpeed = 4f;
    public float RollMovementModifier = 1.5f;

    [Header("DASHING")]
    public float RollStartVelocity = 5f;
    public float RollLengthTime = 0.5f;
    public float RollCoolTime = 0.75f;

    public float RollStartUp = 0.0f;
    public float RollInvlunTime = 0.25f;

    public GameObject JustRollPrefab;

    [Header("ATTACK")]
    public MeleeStatsSO MeleeStats;
    public RangedStatsSO RangedStats;

    [Header("WRITING")]
    public float WritingTime = 1.0f;
    public float WriteSpeedModifier = 0.1f;


}
