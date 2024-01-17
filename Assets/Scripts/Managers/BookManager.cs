using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public string BookRoomType;
    private void Awake(){
        BookData.instance.SetBossList();
        BookData.instance.SetBookList();
    }

    public void SetBookRoomType(string RoomType){
        BookRoomType = RoomType;
    }

    public void SetUnlockedBookLevel(string BookName, int Level){
        BookData.instance.UnlockedBookLevel[BookName] = Level;
    }

    public void SetEquippedBookLevel(string BookName, int Level){
        BookData.instance.EquippedBookLevel[BookName] = Level;
    }

    public void ResetEquippedBookLevel(){
        foreach(string bookName in BookData.instance.EquippedBookLevel.Keys.ToList()){
            BookData.instance.EquippedBookLevel[bookName] = 0;
        }
    }

}
