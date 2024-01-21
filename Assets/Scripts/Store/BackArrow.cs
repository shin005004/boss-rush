using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackArrow : MonoBehaviour
{
    private void OnMouseEnter()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 255 / 255f);
    }

    private void OnMouseOver()
    {
        Debug.Log("Arrow");
        if (Input.GetMouseButtonDown(0))
        {
            SceneLoader.Instance.LoadMainStoreScene();
        }
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
}
