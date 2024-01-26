using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfSetting : MonoBehaviour
{
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject bookObjects;
    [SerializeField] private int bookLevel;
    [SerializeField] private int maxBookLevel;
    public int BookShelfLevel => bookLevel;

    private string roomSetting;
    private List<string> roomBossList;
    private List<GameObject> bookObjectList = new List<GameObject>();

    private void Awake()
    {
        roomSetting = GameManager.Instance.BookManager.BookRoomType;
        roomBossList = BookData.Instance.BossList[roomSetting];
    }

    private float bookY, bookX;
    private GameObject bookObject;
    private void Start(){
        bookY = 3f;
        bookLevel = 1;

        foreach(string boss in roomBossList){
            bookX = -6.5f;
            foreach(string book in BookData.Instance.BookList[roomSetting][boss]){
                bookObject = Instantiate(bookPrefab);
                bookObject.SetActive(false);
                bookObjectList.Add(bookObject);

                if(BookData.Instance.UnlockedBookLevel[book] > 0){
                    if(BookData.Instance.UnlockedBookLevel[book] > bookLevel){
                        bookLevel = BookData.Instance.UnlockedBookLevel[book];
                    }

                    bookObject.SetActive(true);

                    bookObject.transform.SetParent(bookObjects.transform, false);
                    bookObject.transform.position = new Vector3(bookX, bookY, 0f);
                    
                    bookObject.name = book;
                }
                bookX += 1.3f;
            }
            bookY -= 1.6f;
        }

        SetBookLevel();
    }

    private void SetBookLevel(){
        foreach(GameObject bookObject in bookObjectList){
            if(bookLevel <= BookData.Instance.UnlockedBookLevel[bookObject.name]){
                bookObject.SetActive(true);
            }
            else{
                bookObject.SetActive(false);
            }
        }
    }

    public void IncreaseBookLevel(){
        if(bookLevel != maxBookLevel){
            bookLevel += 1;
            SetBookLevel();
        }
    }

    public void DecreaseBookLevel(){
        if(bookLevel != 0){
            bookLevel -= 1;
            SetBookLevel();
        }
    }

}
