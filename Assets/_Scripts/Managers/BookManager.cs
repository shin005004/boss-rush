using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BookManager : MonoBehaviour
{
    public string BookRoomType;



    public void SetBookRoomType(string RoomType){
        BookRoomType = RoomType;
    }

    public void SetUnlockedBookLevel(string BookName, int Level){
        BookData.Instance.UnlockedBookLevel[BookName] = Level;
    }

    public void SetEquippedBookLevel(string BookName, int Level){
        BookData.Instance.EquippedBookLevel[BookName] = Level;
    }

    public void EquipBook(string BookName){
        BookData.Instance.EquippedBook.Add(BookName);
    }
    public void UnequipBook(string BookName){
        BookData.Instance.EquippedBook.Remove(BookName);
        BookData.Instance.EquippedBookLevel[BookName] = 0;
    }

    public void ResetEquippedBookLevel(){
        foreach(string bookName in BookData.Instance.BookNameList){
            BookData.Instance.EquippedBookLevel[bookName] = 0;
            BookData.Instance.EquippedBook = new List<string>() {};
        }
    }

}
