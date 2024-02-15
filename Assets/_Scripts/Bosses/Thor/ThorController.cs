using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThorController : MonoBehaviour, IThorController
{
    #region EXTERNAL
    public Transform ThorVisualTransform;
    public ThorAction CurrentAction => currentActionId;
    public event Action<ThorAction, float> ChangeThorAction;
    public event Action<int, int> AnimationEvent;

    [SerializeField] private Collider2D bossCollider;
    #endregion

    #region VFX
    #endregion

    private int BossHP = 70;
    private PlayerController _player;
    private IPlayerController _playerController;

    private ThorWarner _thorWarner;

    private void Start()
    {
        _player = InstanceManager.Instance.PlayerController;
        _playerController = _player.GetComponent<PlayerController>();
        _player.RollSuccess += OnRollSuccess;

        _thorWarner = GetComponent<ThorWarner>();

        currentActionId = ThorAction.ChantAttack;
    }

    private void Update()
    {
        HandleBrain();

        HandleMove();
        HandleThrowAttack();
        HandleChantAttack();
        HandleFloorAttack();
    }

    #region ThrowAttack

    private bool isThrowing = false;
    [SerializeField] private GameObject[] ThrowObjects;
    [SerializeField] private GameObject[] ThrowWarners;
    private void HandleThrowAttack()
    {
        if (nextActionId != ThorAction.ThrowAttack) return;
        if (isThrowing) return;
        nextActionId = ThorAction.Idle;

        isThrowing = true;

        int randomAction = UnityEngine.Random.Range(0, 1);

        ChangeThorAction?.Invoke(ThorAction.ThrowAttack, 2.5f);
        switch (randomAction)
        {
            case 0:
                StartCoroutine(ThrowAttack1());
                break;
            case 1:
                break;
        }

        
    }

    private IEnumerator ThrowAttack1()
    {
        AudioManager.Instance.PlaySfx(0);
        Instantiate(ThrowObjects[0], transform.position + new Vector3(-0.34f, 1.48f, 0f), Quaternion.identity);
        float totalTime = 1f;

        Quaternion weaponRotation = Quaternion.identity;
        if (BookData.Instance.EquippedBookLevel["Thor2"] == 1)
            ThrowWarners[0].SetActive(true);

        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetDirection = _player.transform.position - (transform.position + new Vector3(-0.34f, 1.48f, 0f));
            float weaponAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);

            ThrowWarners[0].transform.rotation = weaponRotation;

            yield return null;
        }

        stressReceiver.InduceStress(1f);
        if (BookData.Instance.EquippedBookLevel["Thor2"] == 1)
            ThrowWarners[0].SetActive(false);

        if ((_player.transform.position - transform.position).y > 0)
            AnimationEvent?.Invoke(0, 1);
        else
            AnimationEvent?.Invoke(1, 1);

        yield return new WaitForSeconds(1.5f);

        isThrowing = false;
        chooseNextAction = true;

        yield return null;
    }

    #endregion

    #region ChantAttack
    [Header("Chant")]

    private bool isChanting = false;
    [SerializeField] private GameObject[] ChantAttacks;
    [SerializeField] private GameObject[] ChantWarners;

    private int lastChantActionId = 0;
    private int lastChantActionCount = 0;
    private void HandleChantAttack()
    {
        if (nextActionId != ThorAction.ChantAttack) return;
        if (isChanting) return;
        nextActionId = ThorAction.Idle;

        isChanting = true;

        int randomAction = UnityEngine.Random.Range(0, 2);

        if (lastChantActionId == randomAction)
        {
            lastChantActionCount++;

            if (lastChantActionCount > 2)
            {
                randomAction = lastChantActionId = (randomAction == 0) ? 1 : 0;
                lastChantActionCount = 0;
            }
        }
        
        switch (randomAction)
        {
            case 0:
                ChangeThorAction?.Invoke(ThorAction.ChantAttack, 5.5f);
                StartCoroutine(ChantAttack1());
                break;
            case 1:
                ChangeThorAction?.Invoke(ThorAction.ChantAttack, 6f);
                StartCoroutine(ChantAttack2());
                break;
        }
    }

    private IEnumerator ChantAttack1()
    {
        float totalTime = 1f;

        Quaternion weaponRotation = Quaternion.identity;

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[0].SetActive(true);

        for (float elapsedTime = 0; elapsedTime < totalTime; elapsedTime += Time.deltaTime)
        {
            Vector3 targetDirection = _player.transform.position - transform.position;
            float weaponAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);

            ChantWarners[0].transform.rotation = Quaternion.RotateTowards(ChantWarners[0].transform.rotation, weaponRotation, 250f * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[0].SetActive(false);

        AudioManager.Instance.PlaySfx(2);
        stressReceiver.InduceStress(1f);
        Instantiate(ChantAttacks[0], transform.position + new Vector3(0f, -0.4f, 0f), ChantWarners[0].transform.rotation);




        // ------------------------------------------------------

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[0].SetActive(true);

        for (float elapsedTime = 0; elapsedTime < 0.25f; elapsedTime += Time.deltaTime)
        {
            Vector3 targetDirection = _player.transform.position - transform.position;
            float weaponAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);

            ChantWarners[0].transform.rotation = Quaternion.RotateTowards(ChantWarners[0].transform.rotation, weaponRotation, 250f * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[0].SetActive(false);

        stressReceiver.InduceStress(1f);
        AudioManager.Instance.PlaySfx(2);
        Instantiate(ChantAttacks[0], transform.position + new Vector3(0f, -0.4f, 0f), ChantWarners[0].transform.rotation);


        // ------------------------------------------------------------------------

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[0].SetActive(true);

        for (float elapsedTime = 0; elapsedTime < 0.25f; elapsedTime += Time.deltaTime)
        {
            Vector3 targetDirection = _player.transform.position - transform.position;
            float weaponAngle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);

            ChantWarners[0].transform.rotation = Quaternion.RotateTowards(ChantWarners[0].transform.rotation, weaponRotation, 250f * Time.deltaTime);

            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[0].SetActive(false);

        AudioManager.Instance.PlaySfx(2);
        stressReceiver.InduceStress(1f);
        Instantiate(ChantAttacks[0], transform.position + new Vector3(0f, -0.4f, 0f), ChantWarners[0].transform.rotation);

        yield return new WaitForSeconds(3.5f);

        isChanting = false;
        chooseNextAction = true;

        yield return null;
    }
    
    private IEnumerator ChantAttack2()
    {
        AudioManager.Instance.PlaySfx(3);
        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[1].SetActive(true);

        yield return new WaitForSeconds(1f);

        if (BookData.Instance.EquippedBookLevel["Thor1"] == 1)
            ChantWarners[1].SetActive(false);

        AudioManager.Instance.PlaySfx(11);
        stressReceiver.InduceStress(1f);
        Instantiate(ChantAttacks[1], transform.position + new Vector3(0f, -0.7f, 0f), Quaternion.identity);

        yield return new WaitForSeconds(5f);

        isChanting = false;
        chooseNextAction = true;

        yield return null;
    }

    #endregion

    #region HandleMove

    private bool isMoving = false;
    Vector3 moveDestination = Vector3.zero;

    [SerializeField] private GameObject MoveVFX;
    private void HandleMove()
    {
        if (nextActionId != ThorAction.Move) return;
        if (isMoving) return;

        nextActionId = ThorAction.Idle;

        isMoving = true;

        
        float moveRadius = UnityEngine.Random.Range(4f, 7f);
        float moveAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

        moveDestination = new Vector3(moveRadius * Mathf.Cos(moveAngle), moveRadius * Mathf.Sin(moveAngle), 0f);
        while((transform.position - moveDestination).magnitude < 4f)
        {
            moveRadius = UnityEngine.Random.Range(4f, 10f);
            moveAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

            moveDestination = new Vector3(moveRadius * Mathf.Cos(moveAngle), moveRadius * Mathf.Sin(moveAngle), 0f);
        }

        ChangeThorAction?.Invoke(ThorAction.Move, 0.5f);
        StartCoroutine(MoveAction());
    }

    private IEnumerator MoveAction()
    {
        Instantiate(MoveVFX, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.25f);

        //AudioManager.Instance.PlaySfx(8);
        transform.position = moveDestination;
        Instantiate(MoveVFX, transform.position, Quaternion.identity);

        yield return new WaitForSeconds(0.25f);

        isMoving = false;
        chooseNextAction = true;
    }
    #endregion

    #region FloorAttack

    [Header("Floor")]
    [SerializeField] private GameObject[] FloorObjects;
    [SerializeField] private GameObject[] FloorWarners;

    private bool isFloor = false;
    private int lastFloorActionId = 0;
    private int lastFloorActionCount = 0;
    private void HandleFloorAttack()
    {
        if (nextActionId != ThorAction.FloorAttack) return;
        if (isFloor) return;
        nextActionId = ThorAction.Idle;

        isFloor = true;

        int randomAction = UnityEngine.Random.Range(0, 2);

        if (lastFloorActionId == randomAction)
        {
            lastFloorActionCount++;

            if (lastFloorActionCount > 2)
            {
                randomAction = lastFloorActionId = (randomAction == 0) ? 1 : 0;
                lastFloorActionCount = 0;
            }
        }

        switch (randomAction)
        {
            case 0:
                ChangeThorAction?.Invoke(ThorAction.FloorAttack, 4f);
                StartCoroutine(FloorAttack1());
                break;
            case 1:
                ChangeThorAction?.Invoke(ThorAction.FloorAttack, 4f);
                StartCoroutine(FloorAttack2());
                break;
        }
    }

    private IEnumerator FloorAttack1()
    {
        yield return new WaitForSeconds(1f);

        Vector3[] wardPositions = new Vector3[3];
        for (int index = 0; index < 3; index++)
        {
            float spawnRadius = UnityEngine.Random.Range(1f, 4f);
            float spawnAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector3 spawnDestination = new Vector3(spawnRadius * Mathf.Cos(spawnAngle), spawnRadius * Mathf.Sin(spawnAngle), 0f);

            bool distanceFlag = true;
            for (int i = 0; i < index; i++)
            {
                if ((transform.position + spawnDestination - wardPositions[i]).magnitude < 1f)
                    distanceFlag = false;
            }

            while ((transform.position + spawnDestination).magnitude > 7f || !distanceFlag)
            {
                spawnRadius = UnityEngine.Random.Range(4f, 10f);
                spawnAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

                spawnDestination = new Vector3(spawnRadius * Mathf.Cos(spawnAngle), spawnRadius * Mathf.Sin(spawnAngle), 0f);

                distanceFlag = true;
                for (int i = 0; i < index; i++)
                {
                    if ((transform.position + spawnDestination - wardPositions[i]).magnitude < 1f)
                        distanceFlag = false;
                }
            }

            wardPositions[index] = transform.position + spawnDestination;
            Instantiate(FloorObjects[0], transform.position + spawnDestination, Quaternion.identity);
        }

        AudioManager.Instance.PlaySfx(3);
        yield return new WaitForSeconds(1f);
        stressReceiver.InduceStress(1f);

        yield return new WaitForSeconds(2f);

        isFloor = false;
        chooseNextAction = true;

        yield return null;
    }

    private IEnumerator FloorAttack2()
    {
        yield return new WaitForSeconds(1f);

        Vector3[] wardPositions = new Vector3[3];
        for (int index = 0; index < 3; index++)
        {
            float spawnRadius = UnityEngine.Random.Range(1f, 4f);
            float spawnAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

            Vector3 spawnDestination = new Vector3(spawnRadius * Mathf.Cos(spawnAngle), spawnRadius * Mathf.Sin(spawnAngle), 0f);

            bool distanceFlag = true;
            for (int i = 0; i < index; i++)
            {
                if ((transform.position + spawnDestination - wardPositions[i]).magnitude < 1f)
                    distanceFlag = false;
            }

            while ((transform.position + spawnDestination).magnitude > 7f || !distanceFlag)
            {
                spawnRadius = UnityEngine.Random.Range(4f, 10f);
                spawnAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;

                spawnDestination = new Vector3(spawnRadius * Mathf.Cos(spawnAngle), spawnRadius * Mathf.Sin(spawnAngle), 0f);

                distanceFlag = true;
                for (int i = 0; i < index; i++)
                {
                    if ((transform.position + spawnDestination - wardPositions[i]).magnitude < 1f)
                        distanceFlag = false;
                }
            }

            wardPositions[index] = transform.position + spawnDestination;
            Instantiate(FloorObjects[1], transform.position + spawnDestination, Quaternion.identity);
        }

        AudioManager.Instance.PlaySfx(3);
        yield return new WaitForSeconds(3f);


        isFloor = false;
        chooseNextAction = true;

        yield return null;
    }

    #endregion

    #region Brain
    private bool chooseNextAction = true;
    private ThorAction nextActionId = 0;
    private ThorAction currentActionId = 0;

    private int lastActionId = 0;

    public int[] questActionCount = new int[3] { 0, 0, 0 };

    private void HandleBrain()
    {
        if (!chooseNextAction) return;

        chooseNextAction = false;

        switch(currentActionId)
        {
            case ThorAction.Move:

                int nextActionInt = UnityEngine.Random.Range(0, 3);                
                while (nextActionInt == lastActionId)
                    nextActionInt = UnityEngine.Random.Range(0, 3);

                lastActionId = nextActionInt;

                switch (nextActionInt)
                {
                    case 0:
                        currentActionId = nextActionId = ThorAction.ThrowAttack;
                        break;
                    case 1:
                        currentActionId = nextActionId = ThorAction.ChantAttack;
                        break;
                    case 2:
                        currentActionId = nextActionId = ThorAction.FloorAttack;
                        break;
                }
                break;

            case ThorAction.ChantAttack:
                questActionCount[0]++;
                if (questActionCount[0] >= 3)
                    QuestManager.NewQuest("Thor1");

                currentActionId = nextActionId = ThorAction.Move;
                break;
            case ThorAction.ThrowAttack:
                questActionCount[1]++;
                if (questActionCount[1] >= 3)
                    QuestManager.NewQuest("Thor2");

                currentActionId = nextActionId = ThorAction.Move;
                break;
            case ThorAction.FloorAttack:
                questActionCount[2]++;
                if (questActionCount[2] >= 3)
                    QuestManager.NewQuest("Thor3");

                currentActionId = nextActionId = ThorAction.Move;
                break;
        }
    }

    #endregion

    #region HIT
    [Header("HitEFFECT")]
    [SerializeField] private GameObject HitEffect;
    [SerializeField] private StressReceiver stressReceiver;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.transform.name);
        if (collision.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            _playerController.OnBossHit();

            OnBossHit();
            BossHP--;

            Vector3 knockbackDirection = (transform.position - _player.transform.position).normalized;
            float weaponAngle = Mathf.Atan2(knockbackDirection.y, knockbackDirection.x) * Mathf.Rad2Deg + -50f;
            Quaternion weaponRotation = Quaternion.AngleAxis(weaponAngle, Vector3.forward);
            GameObject vfx = Instantiate(HitEffect, transform.position + new Vector3(0, -1f, 0), weaponRotation);

            if (BossHP == 0)
            {
                Debug.Log("BossDeath");
                GameManager.Instance.GameStateManager.ResultState = 2;
            }
               
            Debug.Log("BossHit");
        }
    }

    [SerializeField] private BossSceneUI bossSceneUI;

    private void OnBossHit()
    {
        bossSceneUI.Blood -= 2;
        GameManager.Instance.BloodManager.AddBlood(5);
    }



    private float knockbackStartVelocity = 6f;

    private float knockbackElapsedTime = 0f;
    private float knockbackLengthTime = 0.1f;

    private IEnumerator HitKnockback()
    {
        Vector3 knockbackDirection = (transform.position - _player.transform.position).normalized;

        knockbackElapsedTime = 0f;
        while (knockbackElapsedTime < knockbackLengthTime)
        {
            float rollMovement = Mathf.Lerp(knockbackStartVelocity, knockbackStartVelocity * 0.2f, knockbackElapsedTime / knockbackLengthTime);
            transform.position += knockbackDirection * rollMovement * Time.deltaTime;
            knockbackElapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    #endregion

    #region QUEST

    [SerializeField] private QuestUI QuestManager;
    private void OnRollSuccess()
    {
        if (currentActionId == ThorAction.ThrowAttack)
        {
            QuestManager.NewQuest("Thor2");
        }
        else if (currentActionId == ThorAction.ChantAttack)
        {
            QuestManager.NewQuest("Thor1");
        }
        else
        {
            QuestManager.NewQuest("Thor3");
        }


    }
    #endregion
}

public enum ThorAction
{
    Idle = 0,
    Move = 1,
    ThrowAttack = 2,
    ChantAttack = 3,
    FloorAttack = 4,
}

public interface IThorController
{
    public ThorAction CurrentAction { get; }
    public event Action<ThorAction, float> ChangeThorAction;
    public event Action<int, int> AnimationEvent;
}