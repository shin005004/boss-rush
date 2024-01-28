using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreSceneUI : MonoBehaviour
{
    #region //variables

    public int BloodAmount = 0;
    public int MaxBloodAmout = 200;
    // Those will be changed when the whole blood management structure is completed
    [SerializeField] GameObject bookRightTurn, bookLeftTurn;
    private Animator _bookRightTurnAnim, _bookLeftTurnAnim;
    private VisualElement _popUpLayer, _bag, _scrim, _book, _bloodSprite, _rightArrow, _leftArrow;
    private Label _bloodText;
    private Sprite[] _bloodSprites;
    private int _bloodSpriteCount = 15, _tmpBloodAmout = 0, _turningDirection = -1;

    #endregion
    #region //variables--pages

    private VisualElement _bookSection, _sectionBookIndex, _sectionBookInfo, _sectionBookEquipped;
    private VisualElement[] _indexs = new VisualElement[3], _indexSlots = new VisualElement[8], _equippedSlots = new VisualElement[8];
    private BookUIPage _bookUIPage = new BookUIPage();
    private int _baseSection = 1, _basePage = 1, _tmpSection = 1, _tmpPage = 1;
    private BookUIState _tmpState;
    public static string StoreBookName;
    public static int StoreBookLevel;
    // for interaction with gameobject in store scene
    
    #endregion
    void Awake() 
    {
        _bloodSprites = Resources.LoadAll<Sprite>("Sprites/BloodUI");
        _bloodSpriteCount = _bloodSprites.Length;
        _bookRightTurnAnim = bookRightTurn.GetComponent<Animator>();
        _bookLeftTurnAnim = bookLeftTurn.GetComponent<Animator>();
    }
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _popUpLayer = root.Q<VisualElement>("PopUpLayer");
        _bag = root.Q<VisualElement>("BagSprite");
        _scrim = root.Q<VisualElement>("Scrim");
        _book = root.Q<VisualElement>("BookSprite");
        _bloodSprite = root.Q<VisualElement>("BloodSprite");
        _bloodText = root.Q<Label>("BloodText");
        _rightArrow = root.Q<VisualElement>("RightArrow");
        _leftArrow = root.Q<VisualElement>("LeftArrow");

        _bookSection = root.Q<VisualElement>("BookSection");
        _sectionBookIndex = _bookSection.Q<VisualElement>("Section_BookIndex");
        _sectionBookInfo = _bookSection.Q<VisualElement>("Section_BookInfo");
        _sectionBookEquipped = _bookSection.Q<VisualElement>("Section_BookEquipped");

        _popUpLayer.style.display = DisplayStyle.None;
        _bag.RegisterCallback<ClickEvent>(OnOpenBookForClick);
        _scrim.RegisterCallback<ClickEvent>(OnCloseBook);
        _book.RegisterCallback<TransitionEndEvent>(ClosePopUp);
        _rightArrow.RegisterCallback<ClickEvent>(RightPageTurn);
        _leftArrow.RegisterCallback<ClickEvent>(LeftPageTurn);

        for (int j = 1; j <= 3; j++) {
            _indexs[j - 1] = root.Q<VisualElement>("Index" + j.ToString());
            _indexs[j - 1].RegisterCallback<ClickEvent, VisualElement>(IndexTurn, _indexs[j - 1]);
        }

        _bookUIPage.SetPageList(_bookUIPage.InfoPages, _bookUIPage.Pages);
        _bookSection.RegisterCallback<TransitionEndEvent>(BookTurn);
        _bookSection.AddToClassList("BookSection--Opened");
        ChangeSectionUI();

        for (int i = 1; i <= _bookUIPage.SlotNumber; i++) {
            _indexSlots[i - 1] = _sectionBookIndex.Q<VisualElement>("Slot" + i.ToString());
            _indexSlots[i - 1].RegisterCallback<ClickEvent, VisualElement>(IndexToInfoSection, _indexSlots[i - 1]);
            _equippedSlots[i - 1] = _sectionBookEquipped.Q<VisualElement>("Slot" + i.ToString());
            _equippedSlots[i - 1].RegisterCallback<ClickEvent, VisualElement>(EquippedToInfoSection, _equippedSlots[i - 1]);
        }

        _tmpBloodAmout = BloodAmount;
        ChangeBlood();

        _tmpState = GameManager.Instance.GameStateManager.BookUIState;

        // for test
        //BookData.Instance.EquippedBook.Add("Thor1");
        //BookData.Instance.EquippedBook.Add("Surtur3");
        //BookData.Instance.EquippedBookLevel["Thor1"] = 2;
        //BookData.Instance.EquippedBookLevel["Surtur3"] = 1;

        // for test
        //StoreBookName = "Thor2";
        //StoreBookLevel = 2;
        //GameManager.Instance.GameStateManager.ChangeBookUIState();
    }
    void Update() {
        if (_tmpBloodAmout != BloodAmount) {
            _tmpBloodAmout = BloodAmount;
            ChangeBlood();
        }
        if (bookRightTurn.gameObject.activeSelf) {
            float animTime = _bookRightTurnAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 1.0f) {
                bookRightTurn.gameObject.SetActive(false);
                _bookSection.AddToClassList("BookSection--Opened");
            }
        }
        else if (bookLeftTurn.gameObject.activeSelf) {
            float animTime = _bookLeftTurnAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 1.0f) {
                bookLeftTurn.gameObject.SetActive(false);
                _bookSection.AddToClassList("BookSection--Opened");
            }
        }
        if (_tmpState != GameManager.Instance.GameStateManager.BookUIState) {
            _tmpState = GameManager.Instance.GameStateManager.BookUIState;
            switch (_tmpState) {
                case BookUIState.Guide:
                    Invoke("InformToGuide", 0.5f);
                    break;
                case BookUIState.Inform:
                    GuideToInform();
                    Invoke("OnOpenBook", 0.1f);
                    break;
                
            }
        }
    }
    #region //PopUp
    private void OnOpenBookForClick(ClickEvent evt) 
    {
        OnOpenBook();
    }
    private void OnOpenBook() {
        _bag.AddToClassList("BagSprite--Opened");
        _popUpLayer.style.display = DisplayStyle.Flex;

        GameManager.Instance.GameStateManager.UIOpened = true; // UI State Open
        Debug.Log(GameManager.Instance.GameStateManager.UIOpened);

        Invoke("ActiveBook", 0.1f);
    }
    private void OnCloseBook(ClickEvent evt)
    {
        _bag.RemoveFromClassList("BagSprite--Opened");
        _scrim.RemoveFromClassList("Scrim--FadeIn");
        _book.RemoveFromClassList("BookSprite--Opened");
        if (GameManager.Instance.GameStateManager.BookUIState == BookUIState.Inform) {
            GameManager.Instance.GameStateManager.ChangeBookUIState();
        }
    }
    private void ActiveBook() 
    {
        _scrim.AddToClassList("Scrim--FadeIn");
        _book.AddToClassList("BookSprite--Opened");
    }
    private void ClosePopUp(TransitionEndEvent evt)
    {
        if (!_book.ClassListContains("BookSprite--Opened")) 
        {
            _popUpLayer.style.display = DisplayStyle.None;

            GameManager.Instance.GameStateManager.UIOpened = false; // UI State Close
            Debug.Log(GameManager.Instance.GameStateManager.UIOpened);
        }
    }
    #endregion
    #region //BloodUI
    private void ChangeBlood() 
    {
        for (int i = 0; i < _bloodSpriteCount; i++) 
        {
            int unit = MaxBloodAmout / _bloodSpriteCount;
            if (i == _bloodSpriteCount - 1) {
                if (BloodAmount >= unit * i && BloodAmount <= MaxBloodAmout) {
                    _bloodSprite.style.backgroundImage = new StyleBackground(_bloodSprites[i]);
                }
            }
            if (BloodAmount >= unit * i && BloodAmount < unit * (i + 1)) 
            {
                _bloodSprite.style.backgroundImage = new StyleBackground(_bloodSprites[i]);
            }
        }
        string tmpBloodText = BloodAmount.ToString();
        _bloodText.text = tmpBloodText;
        if (BloodAmount == MaxBloodAmout) {
            _bloodText.style.color = new Color(255/255f, 137/255f, 137/255f);
        }
        else {
            _bloodText.style.color = Color.white;
        }
    }
    #endregion
    #region //BookTurning
    private void BookTurn(TransitionEndEvent transitionEndEvent) {
        if (_turningDirection == 0) {
            _turningDirection = -1;
            ChangeSectionUI();
            BookLeftTurn();
        }
        else if (_turningDirection == 1) {
            _turningDirection = -1;
            ChangeSectionUI();
            BookRightTurn();
        }
    }
    private void IndexTurn(ClickEvent clickEvent, VisualElement visualElement) {
        int index = int.Parse(visualElement.name.Substring(5, 1)) - 1;
        if (_tmpSection > index) {
            _turningDirection = 0;
        }
        else if (_tmpSection < index) {
            _turningDirection = 1;
        }
        else {
            if (_tmpPage > _basePage) {
                _turningDirection = 0;
            }
            else {
                return;
            }
        }
        _tmpSection = index;
        _tmpPage = _basePage;
        _bookSection.RemoveFromClassList("BookSection--Opened");
    }
    private void BookRightTurn() {
        if (!bookRightTurn.gameObject.activeSelf) {
            bookRightTurn.gameObject.SetActive(true);
        }
    }
    private void BookLeftTurn() {
        if (!bookLeftTurn.gameObject.activeSelf) {
            bookLeftTurn.gameObject.SetActive(true);
        }
    }
    private void RightPageTurn(ClickEvent clickEvent) {
        if (!PagePlus()) {
            return;
        }
        _turningDirection = 1;
        _bookSection.RemoveFromClassList("BookSection--Opened");
    }
    private void LeftPageTurn(ClickEvent clickEvent) {
        if (!PageMinus()) {
            return;
        }
        _turningDirection = 0;
        _bookSection.RemoveFromClassList("BookSection--Opened");
    }
    #endregion
    #region //PageUI
    private bool PagePlus() {
        if (_tmpPage == _bookUIPage.Pages[_tmpSection] && _tmpSection != _bookUIPage.Pages.Length - 1) {
            _tmpSection++;
            _tmpPage = 1;
            return true;
        }
        else if (_tmpPage != _bookUIPage.Pages[_tmpSection]) {
            _tmpPage++;
            return true;
        }
        return false;
    }
    private bool PageMinus() {
        if (_tmpPage == 1 && _tmpSection != 0) {
            _tmpSection--;
            _tmpPage = _bookUIPage.Pages[_tmpSection];
            return true;
        }
        else if (_tmpPage != 1) {
            _tmpPage--;
            return true;
        }
        return false;
    }
    private void ChangeSectionUI() {
        _sectionBookIndex.style.display = DisplayStyle.None;
        _sectionBookInfo.style.display = DisplayStyle.None;
        _sectionBookEquipped.style.display = DisplayStyle.None;
        for (int i = 0; i < 3; i++) {
            _indexs[i].style.backgroundImage = StyleKeyword.Null;
        }
        Sprite newIndex = Resources.Load<Sprite>("Sprites/InventoryUI/IndexUI/" + 
        _indexs[_tmpSection].name + "_Selected");
        _indexs[_tmpSection].style.backgroundImage = new StyleBackground(newIndex);
        switch (_tmpSection) {
            case 0:
                _sectionBookIndex.style.display = DisplayStyle.Flex;
                ChangeBookIndexPageUI();
                break;
            case 1:
                _sectionBookInfo.style.display = DisplayStyle.Flex;
                ChangeBookInfoPageUI();
                break;
            case 2:
                _sectionBookEquipped.style.display = DisplayStyle.Flex;
                ChangeBookEquippedPageUI();
                break;
            default:
                break;
        }
    }
    private void ChangeBookIndexPageUI() { // need to be revised
        Sprite[] bookIconSprites = new Sprite[_bookUIPage.InfoPages.Count];
        string[] bookNumberStrings = new string[_bookUIPage.InfoPages.Count];
        string[] bookNumbers = new string[_bookUIPage.InfoPages.Count];
        for (int i = 0; i < _bookUIPage.InfoPages.Count; i++) {
            string spritePath = "Sprites/InventoryUI/BookUI/" + _bookUIPage.InfoPages[i];
            bookIconSprites[i] = Resources.Load<Sprite>(spritePath);
            string bookNumber = (i + 1).ToString();
            bookNumbers[i] = bookNumber;
            switch (bookNumber.Length) {
                case 1:
                    bookNumber = "00" + bookNumber;
                    break;
                case 2:
                    bookNumber = "0" + bookNumber;
                    break;
                default:
                    break;
            }
            bookNumberStrings[i] = bookNumber;
        }

        for (int i = 1; i <= _bookUIPage.SlotNumber; i++) {
            VisualElement slot = _sectionBookIndex.Q<VisualElement>("Slot" + i.ToString());
            int slotIndex = i + (_tmpPage - 1) * _bookUIPage.SlotNumber;

            if (_bookUIPage.InfoPages.Count < slotIndex) {
                slot.style.display = DisplayStyle.None;
                continue;
            }
            slot.style.display = DisplayStyle.Flex;

            string bookFullName = _bookUIPage.InfoPages[slotIndex - 1];
            int bookLevel = int.Parse(bookFullName.Substring(bookFullName.IndexOf("_") + 1, 1));
            string bookName = bookFullName.Substring(0, bookFullName.IndexOf("_"));
            string bookDetailedName = "???";
            Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/Unknown" + bookLevel);
            if (BookData.Instance.UnlockedBookLevel[bookName] == 2) {
                bookIcon = bookIconSprites[slotIndex - 1];
                bookDetailedName = BookData.Instance.BookDetails[bookFullName]["Name"].ToString();
            }
            else if (BookData.Instance.UnlockedBookLevel[bookName] == 1 && bookLevel == 1) {
                bookIcon = bookIconSprites[slotIndex - 1];
                bookDetailedName = BookData.Instance.BookDetails[bookFullName]["Name"].ToString();
            }
        
            Label bookNumberElement = slot.Q<Label>("BookNumber");
            VisualElement bookIconElement = slot.Q<VisualElement>("BookIcon");
            Label bookNameElement = slot.Q<Label>("BookName");
            bookNumberElement.text = bookNumberStrings[slotIndex - 1];
            bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
            slot.viewDataKey = (_bookUIPage.InfoPages.IndexOf(bookFullName) + 1).ToString();
            bookNameElement.text = bookDetailedName;
        }
    }
    private void ChangeBookInfoPageUI() { // need to be revised
        string bookNumber = _tmpPage.ToString();
        switch (bookNumber.Length) {
            case 1:
                bookNumber = "00" + bookNumber;
                break;
            case 2:
                bookNumber = "0" + bookNumber;
                break;
            default:
                break;
        }
        string bookFullName = _bookUIPage.InfoPages[_tmpPage - 1];
        int bookLevel = int.Parse(bookFullName.Substring(bookFullName.IndexOf("_") + 1, 1));
        string bookName = bookFullName.Substring(0, bookFullName.IndexOf("_"));
        string bookDetailedName = "???";
        string bookDetailedBlood = "???";
        string bookDetailedDescription = "???";
        Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/Unknown" + bookLevel);
        if (BookData.Instance.UnlockedBookLevel[bookName] == 2 || BookData.Instance.UnlockedBookLevel[bookName] == 1 && bookLevel == 1) {
            bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/" + bookFullName);
            bookDetailedName = BookData.Instance.BookDetails[bookFullName]["Name"].ToString();
            bookDetailedBlood = BookData.Instance.BookDetails[bookFullName]["Blood"].ToString();
            bookDetailedDescription = BookData.Instance.BookDetails[bookFullName]["Description"].ToString().Replace("\\n", "\n");
        }
        
        Label bookNumberElement = _sectionBookInfo.Q<Label>("BookNumber");
        VisualElement bookIconElement = _sectionBookInfo.Q<VisualElement>("BookIcon");
        Label bookNameElement = _sectionBookInfo.Q<Label>("BookName");
        Label priceTextElement = _sectionBookInfo.Q<Label>("PriceText");
        Label illustrationTextElement = _sectionBookInfo.Q<Label>("IllustrationText");
        bookNumberElement.text = bookNumber;
        bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
        bookNameElement.text = bookDetailedName;
        priceTextElement.text = bookDetailedBlood;
        illustrationTextElement.text = bookDetailedDescription;
    }
    private void ChangeBookEquippedPageUI() { // need to be revised
        for (int i = 1; i <= _bookUIPage.SlotNumber; i++) {
            VisualElement slot = _sectionBookEquipped.Q<VisualElement>("Slot" + i.ToString());
            int slotIndex = i + (_tmpPage - 1) * _bookUIPage.SlotNumber;

            if (BookData.Instance.EquippedBook.Count < slotIndex) {
                slot.style.display = DisplayStyle.None;
                continue;
            }
            slot.style.display = DisplayStyle.Flex;

            string bookName = BookData.Instance.EquippedBook[slotIndex - 1];
            string bookFullName = bookName;
            if (BookData.Instance.EquippedBookLevel[bookName] == 1) {
                bookFullName += "_1";
            }
            else if (BookData.Instance.EquippedBookLevel[bookName] == 2) {
                bookFullName += "_2";
            }
            Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/" + bookFullName);

            Label bookNumberElement = slot.Q<Label>("BookNumber");
            VisualElement bookIconElement = slot.Q<VisualElement>("BookIcon");
            Label bookNameElement = slot.Q<Label>("BookName");
            bookNumberElement.text = slotIndex.ToString();
            bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
            slot.viewDataKey = (_bookUIPage.InfoPages.IndexOf(bookFullName) + 1).ToString();
            bookNameElement.text = BookData.Instance.BookDetails[bookFullName]["Name"].ToString();
        }
    }
    private void IndexToInfoSection(ClickEvent clickEvent, VisualElement visualElement) {
        _tmpSection = 1;
        _tmpPage = int.Parse(visualElement.viewDataKey);
        _turningDirection = 1;
        _bookSection.RemoveFromClassList("BookSection--Opened");
    }
    private void EquippedToInfoSection(ClickEvent clickEvent, VisualElement visualElement) {
        _tmpSection = 1;
        _tmpPage = int.Parse(visualElement.viewDataKey);
        _turningDirection = 0;
        _bookSection.RemoveFromClassList("BookSection--Opened");
    }
    #endregion
    #region //BookUI--guide--inform
    private void GuideToInform() {
        Sprite bookNewSprite = Resources.Load<Sprite>("Sprites/InventoryUI/BookOpened2");
        _book.style.backgroundImage = new StyleBackground(bookNewSprite);
        _leftArrow.style.display = DisplayStyle.None;
        _rightArrow.style.display = DisplayStyle.None;
        for (int i = 0; i < _indexs.Length; i++) {
            _indexs[i].style.display = DisplayStyle.None;
        }
        _tmpSection = 1;
        string bookFullName = StoreBookName;
        if (StoreBookLevel == 1) {
            bookFullName += "_1";
        }
        else if (StoreBookLevel == 2) {
            bookFullName += "_2";
        }
        _tmpPage = _bookUIPage.InfoPages.IndexOf(bookFullName) + 1;
        ChangeSectionUI();
    }
    private void InformToGuide() {
        Sprite bookPreSprite = Resources.Load<Sprite>("Sprites/InventoryUI/BookOpened");
        _book.style.backgroundImage = new StyleBackground(bookPreSprite);
        _leftArrow.style.display = DisplayStyle.Flex;
        _rightArrow.style.display = DisplayStyle.Flex;
        for (int i = 0; i < _indexs.Length; i++) {
            _indexs[i].style.display = DisplayStyle.Flex;
        }
        _tmpSection = _baseSection;
        _tmpPage = _basePage;
        ChangeSectionUI();
    }
    #endregion
}