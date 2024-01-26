using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BookBehaviour : MonoBehaviour
{    

    public string BookName;
    private BookShelfSetting bookShelfSetting;

    private void Start(){
        BookName = gameObject.name;
        bookShelfSetting = GameObject.Find("BookShelf").GetComponent<BookShelfSetting>();
    }

    private void OnMouseEnter()
    {
        gameObject.GetComponent<Renderer>().material.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 255 / 255f);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StoreSceneUI.StoreBookName = BookName;
            StoreSceneUI.StoreBookLevel = bookShelfSetting.BookShelfLevel;
            if(GameManager.Instance.GameStateManager.BookUIState == BookUIState.Guide){
                GameManager.Instance.GameStateManager.ChangeBookUIState();
            }
        }
    }

    private void OnMouseExit()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }
}
