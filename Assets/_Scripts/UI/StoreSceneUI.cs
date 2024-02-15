using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StoreSceneUI : MonoBehaviour
{
    #region //variables

    //public int BloodAmount = 0;
    private int MaxBloodAmout;
    // Those will be changed when the whole blood management structure is completed
    [SerializeField] GameObject bookRightTurn, bookLeftTurn;
    private Animator _bookRightTurnAnim, _bookLeftTurnAnim;
    private VisualElement _popUpLayer, _bag, _scrim, _book, _bloodSprite, _rightArrow, _leftArrow, _closeButton;
    private Label _bloodText;
    private Sprite[] _bloodSprites;
    private int _bloodSpriteCount = 15, _tmpBloodAmout = 0, _turningDirection = -1;

    #endregion
    #region //variables--pages

    private VisualElement _bookSection, _sectionBookIndex, _sectionBookInfo, _sectionBookEquipped;
    private VisualElement[] _indexs = new VisualElement[3], _indexSlots = new VisualElement[8], _equippedSlots = new VisualElement[8];
    private BookUIPage _bookUIPage = new BookUIPage();
    private int _baseSection = 2, _basePage = 1, _tmpSection = 2, _tmpPage = 1;
    private BookUIState _tmpState;
    public static string StoreBookName;
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
        _closeButton = root.Q<VisualElement>("CloseButton");

        _bookSection = root.Q<VisualElement>("BookSection");
        _sectionBookIndex = _bookSection.Q<VisualElement>("Section_BookIndex");
        _sectionBookInfo = _bookSection.Q<VisualElement>("Section_BookInfo");
        _sectionBookEquipped = _bookSection.Q<VisualElement>("Section_BookEquipped");

        _popUpLayer.style.display = DisplayStyle.None;
        _bag.RegisterCallback<ClickEvent>(OnOpenBookForClick);
        _closeButton.RegisterCallback<ClickEvent>(OnCloseBook);
        _scrim.RegisterCallback<TransitionEndEvent>(ClosePopUp);
        _rightArrow.RegisterCallback<ClickEvent>(RightPageTurn);
        _leftArrow.RegisterCallback<ClickEvent>(LeftPageTurn);

        for (int j = 1; j <= 3; j++) {
            _indexs[j - 1] = root.Q<VisualElement>("Index" + j.ToString());
            _indexs[j - 1].RegisterCallback<ClickEvent, VisualElement>(IndexTurn, _indexs[j - 1]);
        }

        _bookUIPage.SetPageList();
        _bookSection.RegisterCallback<TransitionEndEvent>(BookTurn);
        _bookSection.AddToClassList("BookSection--Opened");
        ChangeSectionUI();

        for (int i = 1; i <= _bookUIPage.SlotNumber; i++) {
            _indexSlots[i - 1] = _sectionBookIndex.Q<VisualElement>("Slot" + i.ToString());
            _indexSlots[i - 1].RegisterCallback<ClickEvent, VisualElement>(IndexToInfoSection, _indexSlots[i - 1]);
            _equippedSlots[i - 1] = _sectionBookEquipped.Q<VisualElement>("Slot" + i.ToString());
            _equippedSlots[i - 1].RegisterCallback<ClickEvent, VisualElement>(EquippedToInfoSection, _equippedSlots[i - 1]);
        }

        MaxBloodAmout = GameManager.Instance.BloodManager.MaxBlood;
        _tmpBloodAmout = GameManager.Instance.BloodManager.Blood;
        ChangeBlood();

        _tmpState = GameManager.Instance.GameStateManager.BookUIState;

        // for test
        //BookData.Instance.EquippedBook.Add("Slime1");
        //BookData.Instance.EquippedBook.Add("Slime3");
        //BookData.Instance.EquippedBookLevel["Slime1"] = 1;
        //BookData.Instance.EquippedBookLevel["Slime3"] = 1;

        // for test
        //StoreBookName = "Thor2";
        //StoreBookLevel = 2;
        //GameManager.Instance.GameStateManager.ChangeBookUIState();
    }
    void Update() {
        if (_tmpBloodAmout != GameManager.Instance.BloodManager.Blood) {
            _tmpBloodAmout = GameManager.Instance.BloodManager.Blood;
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
                    InformToGuide();
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
        if (_tmpState == BookUIState.Guide) _bag.AddToClassList("BagSprite--Opened");
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
    }
    private void ActiveBook() 
    {
        _scrim.AddToClassList("Scrim--FadeIn");
        _book.AddToClassList("BookSprite--Opened");
    }
    private void ClosePopUp(TransitionEndEvent evt)
    {
        if (!_scrim.ClassListContains("Scrim--FadeIn")) 
        {
            _popUpLayer.style.display = DisplayStyle.None;

            GameManager.Instance.GameStateManager.UIOpened = false;
            if (GameManager.Instance.GameStateManager.BookUIState == BookUIState.Inform) {
                GameManager.Instance.GameStateManager.ChangeBookUIState();
            }
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
                if (GameManager.Instance.BloodManager.Blood >= unit * i && GameManager.Instance.BloodManager.Blood <= MaxBloodAmout) {
                    _bloodSprite.style.backgroundImage = new StyleBackground(_bloodSprites[i]);
                }
            }
            if (GameManager.Instance.BloodManager.Blood >= unit * i && GameManager.Instance.BloodManager.Blood < unit * (i + 1)) 
            {
                _bloodSprite.style.backgroundImage = new StyleBackground(_bloodSprites[i]);
            }
        }
        string tmpBloodText = GameManager.Instance.BloodManager.Blood.ToString();
        _bloodText.text = tmpBloodText;
        if (GameManager.Instance.BloodManager.Blood == MaxBloodAmout) {
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

            string bookName = _bookUIPage.InfoPages[slotIndex - 1];
            string bookDetailedName = "???";
            Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/Unknown1");
            if (BookData.Instance.UnlockedBookLevel[bookName] == 1) {
                bookIcon = bookIconSprites[slotIndex - 1];
                bookDetailedName = BookData.Instance.BookDetails[bookName]["Name"].ToString();
            }
        
            Label bookNumberElement = slot.Q<Label>("BookNumber");
            VisualElement bookIconElement = slot.Q<VisualElement>("BookIcon");
            Label bookNameElement = slot.Q<Label>("BookName");
            bookNumberElement.text = bookNumberStrings[slotIndex - 1];
            bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
            slot.viewDataKey = (_bookUIPage.InfoPages.IndexOf(bookName) + 1).ToString();
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
        string bookName = _bookUIPage.InfoPages[_tmpPage - 1];
        string bookDetailedName = "???";
        string bookDetailedBlood = "???";
        string bookDetailedDescription = "???";
        Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/Unknown1");
        Sprite bookIllustration = Resources.Load<Sprite>("Sprites/InventoryUI/Illustration/Unknown");
        if (BookData.Instance.UnlockedBookLevel[bookName] == 1) {
            bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/" + bookName);
            bookIllustration = Resources.Load<Sprite>("Sprites/InventoryUI/Illustration/" + bookName);
            bookDetailedName = BookData.Instance.BookDetails[bookName]["Name"].ToString();
            bookDetailedBlood = BookData.Instance.BookDetails[bookName]["Blood"].ToString();
            bookDetailedDescription = BookData.Instance.BookDetails[bookName]["Description"].ToString().Replace("\\n", "\n");
        }
        
        Label bookNumberElement = _sectionBookInfo.Q<Label>("BookNumber");
        VisualElement bookIconElement = _sectionBookInfo.Q<VisualElement>("BookIcon");
        Label bookNameElement = _sectionBookInfo.Q<Label>("BookName");
        Label priceTextElement = _sectionBookInfo.Q<Label>("PriceText");
        VisualElement bookIllustrationElement = _sectionBookInfo.Q<VisualElement>("Illustration");
        Label illustrationTextElement = _sectionBookInfo.Q<Label>("IllustrationText");
        bookNumberElement.text = bookNumber;
        bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
        bookIllustrationElement.style.backgroundImage = new StyleBackground(bookIllustration);
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
            Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/" + bookName);

            Label bookNumberElement = slot.Q<Label>("BookNumber");
            VisualElement bookIconElement = slot.Q<VisualElement>("BookIcon");
            Label bookNameElement = slot.Q<Label>("BookName");
            bookNumberElement.text = slotIndex.ToString();
            bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
            slot.viewDataKey = (_bookUIPage.InfoPages.IndexOf(bookName) + 1).ToString();
            bookNameElement.text = BookData.Instance.BookDetails[bookName]["Name"].ToString();
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
        _tmpPage = _bookUIPage.InfoPages.IndexOf(StoreBookName) + 1;
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