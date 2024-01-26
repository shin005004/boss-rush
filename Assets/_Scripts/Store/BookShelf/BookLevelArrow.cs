using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookLevelArrow : MonoBehaviour
{
    [SerializeField] private string arrowType;
    private BookShelfSetting bookShelfSetting;


    private void Start(){
        bookShelfSetting = GameObject.Find("BookShelf").GetComponent<BookShelfSetting>();
    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 255 / 255f);
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(arrowType == "Left"){
                bookShelfSetting.DecreaseBookLevel();
            }
            else if(arrowType == "Right"){
                bookShelfSetting.IncreaseBookLevel();    
            }
        }
    }
}
