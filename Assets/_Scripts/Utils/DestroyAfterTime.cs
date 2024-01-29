using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private GameObject objectToDestroy;
    [SerializeField] private bool destroyAfterTime;
    [SerializeField] private float destroyTime;

    [Header("WeaponHitbox")]
    public Transform[] colliderTransforms;

    private void Start()
    {
        if (objectToDestroy == null)
        {
            Debug.LogError("ObjectToDestroy is null");
            objectToDestroy = this.gameObject;
        }
        totalTime = 0;
    }

    private float totalTime = 0f;
    private void Update()
    {
        totalTime += Time.deltaTime;
        if (destroyAfterTime && destroyTime < totalTime)
        {
            Destroy(objectToDestroy);
        }
    }

    public void SetColliderOn(int index)
    {
        var colliderList = colliderTransforms[index].GetComponents<Collider2D>();
        foreach (var collider in colliderList)
        {
            collider.enabled = true;
        }
    }

    public void SetColliderOff(int index)
    {
        var colliderList = colliderTransforms[index].GetComponents<Collider2D>();
        foreach (var collider in colliderList)
        {
            collider.enabled = false;
        }
    }

    public void DestroyObject()
    {
        Destroy(objectToDestroy);
    }
}
