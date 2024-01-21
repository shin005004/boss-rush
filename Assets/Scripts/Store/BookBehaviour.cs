using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookBehaviour : MonoBehaviour
{    

    public string BookName;

    private void Start(){
        BookName = gameObject.name;
    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 255 / 255f);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
        }
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
}
