using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustRollDummy : MonoBehaviour
{
    private float _rollStartUpTime;
    private float _rollInvulnTime;
    private IPlayerController _playerController;

    public void StartUp(float startUpTime, float invulvTime, IPlayerController playerController)
    {
        _rollStartUpTime = startUpTime;
        _rollInvulnTime = invulvTime;
        this._playerController = playerController;
    }

    private float rollElapsedTime = 0f;
    private void Update()
    {
        rollElapsedTime += Time.deltaTime;
        if (_rollInvulnTime < rollElapsedTime)
            Destroy(gameObject);
    }

    private bool isRollSuccess = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("BossHitbox"))
        {
            if (rollElapsedTime > _rollStartUpTime && rollElapsedTime < _rollInvulnTime && !isRollSuccess)
            {   
                isRollSuccess = true;
                _playerController.OnRollSuccess();
                Destroy(gameObject);
            }
        }
    }
}
