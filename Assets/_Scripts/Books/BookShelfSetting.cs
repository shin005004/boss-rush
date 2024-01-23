using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfSetting : MonoBehaviour
{
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject bookObjects;

    private string roomSetting;
    private List<string> roomBossList;

    private void Awake()
    {
        roomSetting = GameManager.Instance.BookManager.BookRoomType;
        roomBossList = BookData.Instance.BossList[roomSetting];
    }

    private float bookY, bookX;
    private GameObject bookObject;
    private void Start(){
        bookY = 3f;

        foreach(string boss in roomBossList){
            bookX = -6.5f;
            foreach(string book in BookData.Instance.BookList[roomSetting][boss]){
                if(BookData.Instance.UnlockedBookLevel[book] > 0){
                    bookObject = Instantiate(bookPrefab);
                    
                    bookObject.transform.SetParent(bookObjects.transform, false);
                    bookObject.transform.position = new Vector3(bookX, bookY, 0f);
                    
                    bookObject.name = book;

                    bookX += 1.3f;
                }
            }
            bookY -= 1.6f;
        }
    }
    
}
