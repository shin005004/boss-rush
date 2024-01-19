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
    public float DashMovementModifier = 1.5f;

    [Header("DASHING")]
    public float DashStartVelocity = 5f;
    public float DashLengthTime = 0.5f;
    public float DashCoolTime = 0.75f;

    public float DashStartUp = 0.0f;
    public float DashInvlunTime = 0.25f;

    public GameObject JustDashPrefab;

    [Header("ATTACK")]
    public MeleeStatsSO MeleeStats;
    public RangedStatsSO RangedStats;

    [Header("WRITING")]
    public float WritingTime = 1.0f;
    public float WriteSpeedModifier = 0.1f;


}
