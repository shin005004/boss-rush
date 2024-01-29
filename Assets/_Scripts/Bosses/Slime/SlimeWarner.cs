using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeWarner : MonoBehaviour
{
    [SerializeField] private GameObject BigJumpWarner;
    [SerializeField] private GameObject StompWarner;
    [SerializeField] private GameObject ShockwaveWarner;

    public void ShowBigjumpWarner(Vector2 warnerPosition, bool showWarner)
    {
        BigJumpWarner.transform.position = warnerPosition;
        BigJumpWarner.SetActive(showWarner);
    }

    public void ShowStompWarner(Vector2 warnerPosition, bool showWarner)
    {
        StompWarner.transform.position = warnerPosition;
        StompWarner.SetActive(showWarner);
    }

    public void ShowShockwaveWarner(Vector2 warnerPosition, bool showWarner)
    {
        ShockwaveWarner.transform.position = warnerPosition;
        ShockwaveWarner.SetActive(showWarner);
    }
}
