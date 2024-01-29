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
        BigJumpWarner.transform.position = warnerPosition + new Vector2(0f, -0.42f);
        BigJumpWarner.SetActive(showWarner);
    }

    public void ShowStompWarner(Vector2 warnerPosition, bool showWarner)
    {
        StompWarner.transform.position = warnerPosition;
        StompWarner.SetActive(showWarner);
    }

    public void ShowShockwaveWarner(Vector2 warnerPosition)
    {
        ShockwaveWarner.transform.position = warnerPosition;

        ShockwaveWarner.GetComponent<Animator>().SetTrigger("WarnerActive");
    }
}
