using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThorWardController : MonoBehaviour
{
    [SerializeField] private int Id = 0;
    [SerializeField] private float HoldTime = 2f;

    [SerializeField] private GameObject BlueBeam;
    [SerializeField] private GameObject Warner;

    private bool setFlag = true;
    private float totalTime = 0f;

    GameObject warner1, warner2;
    private void Start()
    {
        if (BookData.Instance.EquippedBookLevel["Thor3"] == 1)
        {
            if (Id == 0)
            {
                warner1 = Instantiate(Warner, transform.position, Quaternion.Euler(0f, 0f, 0f));
                warner2 = Instantiate(Warner, transform.position, Quaternion.Euler(0f, 0f, 90f));
            }
            if (Id == 1)
            {
                warner1 = Instantiate(Warner, transform.position, Quaternion.Euler(0f, 0f, 45f));
                warner2 = Instantiate(Warner, transform.position, Quaternion.Euler(0f, 0f, 135f));
            }
        }
        
    }

    private void FixedUpdate()
    {
        totalTime += Time.deltaTime;
        

        if (HoldTime < totalTime && setFlag)
        {
            if (BookData.Instance.EquippedBookLevel["Thor3"] == 1)
            {
                Destroy(warner1);
                Destroy(warner2);
            }

            if (Id == 0)
            {
                AudioManager.Instance.PlaySfx(2);
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 0f));
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 90f));
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 180f));
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 270f));
            }

            if (Id == 1)
            {
                AudioManager.Instance.PlaySfx(2);
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 45f));
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 135f));
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 225f));
                Instantiate(BlueBeam, transform.position, Quaternion.Euler(0f, 0f, 315f));
            }

            setFlag = false;
        }
    }
}
