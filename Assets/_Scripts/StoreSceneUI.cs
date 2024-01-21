using System;
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
    private VisualElement _popUpLayer, _bag, _scrim, _book, _bloodSprite, _rightArrow, _leftArrow, _rightIndex, _leftIndex;
    private Label _bloodText;
    private Sprite[] _bloodSprites;
    private int _bloodSpriteCount = 15;
    private int _tmpBloodAmout = 0;
    private int _turningDirection = -1;
    #endregion
    #region //variables--pages
    private VisualElement _sectionBookIndex, _sectionBookInfo, _sectionBookEquipped;
    private BookUIPage _bookUIPage = new BookUIPage();
    private int _tmpSection = 0, _tmpPage = 1;
    #endregion
    void Awake() 
    {
        _bloodSprites = Resources.LoadAll<Sprite>("Sprites/BloodUI");
        _bloodSpriteCount = _bloodSprites.Length;
        _bookRightTurnAnim = bookRightTurn.GetComponent<Animator>();
        _bookLeftTurnAnim = bookLeftTurn.GetComponent<Animator>();

        _bookUIPage.SetPageList();
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
        _rightIndex = root.Q<VisualElement>("RightIndex");
        _leftIndex = root.Q<VisualElement>("LeftIndex");
        _sectionBookInfo = root.Q<VisualElement>("Section_BookInfo");

        _popUpLayer.style.display = DisplayStyle.None;
        _bag.RegisterCallback<ClickEvent>(OnOpenBook);
        _scrim.RegisterCallback<ClickEvent>(OnCloseBook);
        _book.RegisterCallback<TransitionEndEvent>(ClosePopUp);
        _rightArrow.RegisterCallback<ClickEvent>(RightPageTurn);
        _leftArrow.RegisterCallback<ClickEvent>(LeftPageTurn);
        _rightIndex.RegisterCallback<ClickEvent>(RightIndexTurn);
        _leftIndex.RegisterCallback<ClickEvent>(LeftIndexTurn);

        _sectionBookInfo.RegisterCallback<TransitionEndEvent>(BookTurn);

        _tmpBloodAmout = BloodAmount;
        ChangeBlood();
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
                // for test
                _sectionBookInfo.AddToClassList("Section_BookInfo--Opened");
            }
        }
        else if (bookLeftTurn.gameObject.activeSelf) {
            float animTime = _bookLeftTurnAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            if (animTime >= 1.0f) {
                bookLeftTurn.gameObject.SetActive(false);
                // for test
                _sectionBookInfo.AddToClassList("Section_BookInfo--Opened");
            }
        }
    }
    #region //PopUp
    private void OnOpenBook(ClickEvent evt) 
    {
        _bag.AddToClassList("BagSprite--Opened");
        _popUpLayer.style.display = DisplayStyle.Flex;

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
        if (!_book.ClassListContains("BookSprite--Opened")) 
        {
            _popUpLayer.style.display = DisplayStyle.None;
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
            BookLeftTurn();
        }
        else if (_turningDirection == 1) {
            _turningDirection = -1;
            BookRightTurn();
        }
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
    private void RightIndexTurn(ClickEvent clickEvent) {
        _turningDirection = 1;
        if (_sectionBookInfo.ClassListContains("Section_BookInfo--Opened")) {
            _sectionBookInfo.RemoveFromClassList("Section_BookInfo--Opened");
        }
        else {
            _turningDirection = -1;
            BookRightTurn();
        }
        // TODO: change section variables
        // change page variables
    }
    private void LeftIndexTurn(ClickEvent clickEvent) {
        _turningDirection = 0;
        if (_sectionBookInfo.ClassListContains("Section_BookInfo--Opened")) {
            _sectionBookInfo.RemoveFromClassList("Section_BookInfo--Opened");
        }
        else {
            _turningDirection = -1;
            BookLeftTurn();
        }
        // TODO: change section variables
        // change page variables
    }
    private void RightPageTurn(ClickEvent clickEvent) {
        _turningDirection = 1;
        if (_sectionBookInfo.ClassListContains("Section_BookInfo--Opened")) {
            _sectionBookInfo.RemoveFromClassList("Section_BookInfo--Opened");
        }
        else {
            _turningDirection = -1;
            BookRightTurn();
        }
        // TODO: change page variables
    }
    private void LeftPageTurn(ClickEvent clickEvent) {
        _turningDirection = 0;
        if (_sectionBookInfo.ClassListContains("Section_BookInfo--Opened")) {
            _sectionBookInfo.RemoveFromClassList("Section_BookInfo--Opened");
        }
        else {
            _turningDirection = -1;
            BookLeftTurn();
        }
        // TODO: change page variables
    }
    #endregion
    #region //PageUI
    private void PagePlus() {
        if (_tmpPage == _bookUIPage.Pages[_tmpSection] && _tmpSection != _bookUIPage.Pages.Length) {
            _tmpSection++;
            ChangeSectionUI();
            _tmpPage = 1;
        }
        else if (_tmpPage != _bookUIPage.Pages[_tmpSection]) {
            _tmpPage++;
        }
    }
    private void PageMinus() {
        if (_tmpPage == 1 && _tmpSection != 0) {
            _tmpSection--;
            ChangeSectionUI();
            _tmpPage = _bookUIPage.Pages[_tmpSection];
        }
        else if (_tmpPage != 1) {
            _tmpPage--;
        }
    }
    private void ChangeSectionUI() {
        _sectionBookIndex.style.display = DisplayStyle.None;
        _sectionBookInfo.style.display = DisplayStyle.None;
        _sectionBookEquipped.style.display = DisplayStyle.None;
        switch (_tmpSection) {
            case 0:
                _sectionBookIndex.style.display = DisplayStyle.Flex;
                break;
            case 1:
                _sectionBookInfo.style.display = DisplayStyle.Flex;
                break;
            case 2:
                _sectionBookEquipped.style.display = DisplayStyle.Flex;
                break;
            default:
                break;
        }
    }
    private void ChangePageUI() {
        if (_tmpSection == 0) {

        }
        else if (_tmpSection == 1) {
            ChangeBookInfoPageUI();
        }
        else if (_tmpSection == 2) {

        }
    }
    private void ChangeBookInfoPageUI() {
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
        string bookFullName = _bookUIPage.InfoPages[_tmpPage];
        int bookLevel = int.Parse(bookFullName.Substring(bookFullName.IndexOf("_") + 1, 1));
        string bookName = bookFullName.Substring(0, bookFullName.IndexOf("_"));
        Sprite bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/Unknown" + bookLevel);
        if (BookData.Instance.UnlockedBookLevel[bookName] == 2) {
            bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/" + bookFullName);
        }
        else if (BookData.Instance.UnlockedBookLevel[bookName] == 1) {
            if (bookLevel == 1) {
                bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/" + bookFullName);
            }
            else if (bookLevel == 2) {
                bookIcon = Resources.Load<Sprite>("Sprites/InventoryUI/BookUI/Unknown" + bookLevel);
            }
        }
        
        Label bookNumberElement = _sectionBookInfo.Q<Label>("BookNumber");
        VisualElement bookIconElement = _sectionBookInfo.Q<VisualElement>("BookIcon");
        bookNumberElement.text = bookNumber;
        bookIconElement.style.backgroundImage = new StyleBackground(bookIcon);
    }
    #endregion
}
