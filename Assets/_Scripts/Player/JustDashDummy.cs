using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JustDashDummy : MonoBehaviour
{
    private float _dashStartUpTime;
    private float _dashInvulnTime;
    private IPlayerController _playerController;

    public void StartUp(float startUpTime, float invulvTime, IPlayerController playerController)
    {
        _dashStartUpTime = startUpTime;
        _dashInvulnTime = invulvTime;
        this._playerController = playerController;
    }

    private float dashElapsedTime = 0f;
    private void Update()
    {
        dashElapsedTime += Time.deltaTime;
        if (_dashStartUpTime + _dashInvulnTime < dashElapsedTime)
            Destroy(gameObject);
    }

    private bool isDashSuccess = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("DeathHitBox"))
        {
            if (dashElapsedTime > _dashStartUpTime && dashElapsedTime < _dashStartUpTime + _dashInvulnTime && !isDashSuccess)
            {   
                isDashSuccess = true;
                _playerController.OnDashSuccess();
                Destroy(gameObject);
            }
        }
    }
}
