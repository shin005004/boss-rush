using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] private GameObject bookNameObject;
    [SerializeField] private TextMeshProUGUI bookNameText;
    private RectTransform bookNameTransform;
    public float BookX, BookY;
    public string BookName;
    public bool ShowBookName;


    private string roomSetting;
    private List<string> roomBossList;
    [SerializeField] private List<GameObject> bookObjectList = new List<GameObject>();

    private void Awake()
    {
        roomSetting = GameManager.Instance.BookManager.BookRoomType;
        roomBossList = BossData.Instance.BossList[roomSetting];
        bookNameTransform = bookNameObject.GetComponent<RectTransform>();
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
        SetEquippedBookInActive();
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
    }
    
    private void SetEquippedBookInActive() {
        foreach (GameObject bookObject in bookObjectList) {
            if (BookData.Instance.EquippedBookLevel[bookObject.name] == 1) {
                bookObject.GetComponent<Image>().color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 1f);
            }
        }
    }
    private void Update(){
        if(ShowBookName){
            bookNameObject.SetActive(true);
            bookNameTransform.anchoredPosition = new Vector2(BookX, BookY + 30f);
            bookNameText.text = BookName;
        }
        else{
            bookNameObject.SetActive(false);
        }
    }

}
