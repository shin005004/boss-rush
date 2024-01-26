using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfSetting : MonoBehaviour
{
    [SerializeField] private GameObject bookPrefab;
    [SerializeField] private GameObject bookObjects;
    [SerializeField] private int bookShelfLevel;
    [SerializeField] private int maxBookLevel;
    [SerializeField] private GameObject bookShelfLevelObject;
    [SerializeField] private Sprite[] bookShelfLevelSprites;
    public int BookShelfLevel => bookShelfLevel;

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
        bookY = 3f;
        bookShelfLevel = 1;

        foreach(string boss in roomBossList){
            bookX = -6.5f;
            foreach(string book in BookData.Instance.BookList[roomSetting][boss]){
                bookObject = Instantiate(bookPrefab);
                bookObject.name = book;
                bookObjectList.Add(bookObject);
                bookObject.SetActive(false);
                bookObject.transform.SetParent(bookObjects.transform, false);

                if(BookData.Instance.UnlockedBookLevel[book] > 0){
                    if(BookData.Instance.UnlockedBookLevel[book] > bookShelfLevel){
                        bookShelfLevel = BookData.Instance.UnlockedBookLevel[book];
                    }

                    bookObject.SetActive(true);
                    bookObject.transform.position = new Vector3(bookX, bookY, 0f);
                    
                }
                bookX += 1.3f;
            }
            bookY -= 1.6f;
        }

        SetBookLevel();
    }

    private void SetBookLevel(){
        foreach(GameObject bookObject in bookObjectList){
            if(bookShelfLevel <= BookData.Instance.UnlockedBookLevel[bookObject.name]){
                bookObject.SetActive(true);
            }
            else{
                bookObject.SetActive(false);
            }
        }

        bookShelfLevelObject.GetComponent<SpriteRenderer>().sprite = bookShelfLevelSprites[bookShelfLevel-1];
    }

    public void IncreaseBookLevel(){
        if(bookShelfLevel != maxBookLevel){
            bookShelfLevel += 1;
            SetBookLevel();
        }
    }

    public void DecreaseBookLevel(){
        if(bookShelfLevel != 1){
            bookShelfLevel -= 1;
            SetBookLevel();
        }
    }

}
