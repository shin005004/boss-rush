using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThrowObjectController : MonoBehaviour
{
    [SerializeField] private float HoldTime = 4f;
    [SerializeField] private float LifeTime = 30f;
    [SerializeField] private float ProjectileSpeed = 10f;

    private void Start()
    {
        
    }

    private bool setFlag = true;
    private float totalTime = 0f;

    Vector3 direction;
    private void FixedUpdate()
    {
        totalTime += Time.deltaTime;
        if (LifeTime < totalTime)
        {
            Destroy(gameObject);
        }

        if (HoldTime < totalTime && setFlag)
        {
            Vector3 targetPosition = InstanceManager.Instance.PlayerController.transform.position;
            direction = targetPosition - transform.position;

            direction = direction.normalized;

            setFlag = false;
        }
        if (HoldTime < totalTime)
        {
            transform.position = transform.position + ProjectileSpeed * Time.deltaTime * direction;
        }
    }
}
