using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class BookBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string BookName;
    private Image image;
    private Transform bookShelf;
    private BookShelfSetting bookShelfSetting;
    private RectTransform rectTransform;

    private void Start()
    {
        BookName = gameObject.name;
        image = gameObject.GetComponent<Image>();
        bookShelf = transform.parent.parent;
        bookShelfSetting = bookShelf.GetComponent<BookShelfSetting>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        image.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 1f);
        bookShelfSetting.BookName = BookName;
        bookShelfSetting.BookX = rectTransform.anchoredPosition.x;
        bookShelfSetting.BookY = rectTransform.anchoredPosition.y;
        bookShelfSetting.ShowBookName = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            StoreSceneUI.StoreBookName = BookName;

            if (GameManager.Instance.GameStateManager.BookUIState == BookUIState.Guide)
            {
                GameManager.Instance.GameStateManager.ChangeBookUIState();
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if(BookData.Instance.EquippedBookLevel[BookName] == 0){
                int neededBlood = Convert.ToInt32(BookData.Instance.BookDetails[BookName]["Blood"]);
                if(GameManager.Instance.BloodManager.Blood >= neededBlood){
                    BookData.Instance.EquippedBookLevel[BookName] = 1;
                    GameManager.Instance.BloodManager.UseBlood(neededBlood);
                    if(!BookData.Instance.EquippedBook.Contains(BookName)){ BookData.Instance.EquippedBook.Add(BookName); }
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (BookData.Instance.EquippedBookLevel[BookName] < 1){
            image.color = Color.white;
        }
        else{
            image.color = new Color(169 / 255f, 169 / 255f, 169 / 255f, 1f);
        }
        
        bookShelfSetting.ShowBookName = false;
    }
}