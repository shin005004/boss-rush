using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using UnityEngine;
using System;

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
            // StoreSceneUI.StoreBookLevel = bookShelfSetting.BookShelfLevel;
            if(GameManager.Instance.GameStateManager.BookUIState == BookUIState.Guide){
                GameManager.Instance.GameStateManager.ChangeBookUIState();
            }
        }
        else if (Input.GetMouseButtonDown(1)){
            if(BookData.Instance.EquippedBookLevel[BookName] == 0){
                int neededBlood = Convert.ToInt32(BookData.Instance.BookDetails[BookName]["Blood"]);
                if(GameManager.Instance.BloodManager.Blood >= neededBlood){
                    BookData.Instance.EquippedBookLevel[BookName] = 1;
                    GameManager.Instance.BloodManager.UseBlood(neededBlood);
                    if(!BookData.Instance.EquippedBook.Contains(BookName)){ BookData.Instance.EquippedBook.Add(BookName); }
                }
            }
            // else if(BookData.Instance.EquippedBookLevel[BookName] < bookShelfSetting.BookShelfLevel){
            //     string equippedBookNameLevel = BookName + "_" + BookData.Instance.EquippedBookLevel[BookName].ToString();
            //     int neededBlood = Convert.ToInt32(BookData.Instance.BookDetails[bookNameLevel]["Blood"]) - Convert.ToInt32(BookData.Instance.BookDetails[equippedBookNameLevel]["Blood"]);
            //     if(GameManager.Instance.BloodManager.Blood >= neededBlood){
            //         BookData.Instance.EquippedBookLevel[BookName] = bookShelfSetting.BookShelfLevel;
            //         GameManager.Instance.BloodManager.UseBlood(neededBlood);
            //         if(!BookData.Instance.EquippedBook.Contains(BookName)){ BookData.Instance.EquippedBook.Add(BookName); }
            //     }
            // }
        }
    }

    private void OnMouseExit()
    {
        if(BookData.Instance.EquippedBookLevel[BookName] < 1) gameObject.GetComponent<Renderer>().material.color = Color.white;
        else gameObject.GetComponent<Renderer>().material.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 255 / 255f);
    }
}
