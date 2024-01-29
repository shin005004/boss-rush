using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfSetting : MonoBehaviour
{
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject bookObjects;
    [SerializeField] private Transform canvasTransform;
    [SerializeField] private int bookShelfLevel;
    [SerializeField] private int maxBookLevel;
    // [SerializeField] private GameObject bookShelfLevelObject;
    // [SerializeField] private Sprite[] bookShelfLevelSprites;
    // public int BookShelfLevel => bookShelfLevel;

    private RectTransform bookTransform;

    private string roomSetting;
    private List<string> roomBossList;
    [SerializeField] private List<GameObject> bookObjectList = new List<GameObject>();

    private void Awake()
    {
        roomSetting = GameManager.Instance.BookManager.BookRoomType;
        roomBossList = BookData.Instance.BossList[roomSetting];
    }

    private float bookY, bookX;
    private GameObject bookObject;
    private void Start(){
        bookY = 115f;
        bookShelfLevel = 1;

        foreach(string boss in roomBossList){
            bookX = -200f;
            foreach(string book in BookData.Instance.BookList[roomSetting][boss]){
                bookObject = Instantiate(bookPrefab, bookObjects.transform, false);
                bookObject.name = book;
                bookObjectList.Add(bookObject);
                bookObject.SetActive(false);

                bookTransform = bookObject.GetComponent<RectTransform>();
                bookTransform.anchoredPosition = new Vector2(bookX, bookY);

                if(BookData.Instance.UnlockedBookLevel[book] > 0){ bookObject.SetActive(true); }

                bookX += 60f;
            }
            bookY -= 60f;
        }

        SetBookActive();
    }

    private void SetBookActive(){
        foreach(GameObject bookObject in bookObjectList){
            if(bookShelfLevel <= BookData.Instance.UnlockedBookLevel[bookObject.name]){
                bookObject.SetActive(true);
            }
            else{
                bookObject.SetActive(false);
            }
        }

        // bookShelfLevelObject.GetComponent<SpriteRenderer>().sprite = bookShelfLevelSprites[bookShelfLevel-1];
    }

    // public void IncreaseBookLevel(){
    //     if(bookShelfLevel != maxBookLevel){
    //         bookShelfLevel += 1;
    //         SetBookLevel();
    //     }
    // }

    // public void DecreaseBookLevel(){
    //     if(bookShelfLevel != 1){
    //         bookShelfLevel -= 1;
    //         SetBookLevel();
    //     }
    // }

}
