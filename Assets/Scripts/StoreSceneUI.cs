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
    private VisualElement _popUpLayer, _bag, _scrim, _book, _bloodSprite, _rightArrow, _leftArrow, _rightIndex, _leftIndex, _sectionBookInfo;
    private Label _bloodText;
    private Sprite[] _bloodSprites;
    private int _bloodSpriteCount = 15;
    private int _tmpBloodAmout = 0;
    private int _turningDirection = -1;
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
    // TODO
    // (O) Change blood sprites, load the previous font in blood UI and adjust 
    // () Adjust the close book called area (sprite editing)
    // (O) Make the page turn: arrows, animation, index transition
    // () Show illustrated guide and equipped books based on arbitrary csv file or data info
    // () Create explanation pop-up about each slot in illustrated guide book.
}
