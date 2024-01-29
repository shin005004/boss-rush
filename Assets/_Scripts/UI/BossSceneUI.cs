using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class BossSceneUI : MonoBehaviour
{
    #region //element
    [SerializeField] GameObject _bulletPrefab, _bulletCanvas;
    private VisualElement _bossBlood, _bloodBar_Forward;
    private Label _bossNameText, _equippedCountText;
    private VisualElement[] _questPanels = new VisualElement[3];
    private Label[] _questPanelTexts = new Label[3];
    #endregion
    #region //variables
    public string BossName = "Boss"; // need to be connected to map data
    public int FullBlood = 200, Blood = 200; // need to be connected to map data
    public int FullBullet = 4, Bullet = 1; // need to be connected to bullet data
    public int Margin = 70; // this is for UI position adjusting
    private int _tmpBlood, _equippedCount, _tmpBullet, _bulletX = -50, _bulletY = 50;
    #endregion
    void Awake() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _bossBlood = root.Q<VisualElement>("BossBlood");
        _bloodBar_Forward = root.Q<VisualElement>("BloodBar_Forward");
        _bossNameText = root.Q<Label>("BossName");
        _equippedCountText = root.Q<Label>("EquippedCount");
        for (int i = 0; i < 3; i++) {
            _questPanels[i] = root.Q<VisualElement>("Panel" + (i + 1).ToString());
            _questPanelTexts[i] = _questPanels[i].Q<Label>("PanelText");
        }
    }
    void Start()
    {   
        GameManager.Instance.GameStateManager.UIOpened = false;
        _equippedCount = BookData.Instance.EquippedBook.Count;
        _tmpBlood = Blood;
        SetBossBlood();
        SetBossSceneUI();

        Invoke("BossBloodAppear", 0.5f);

        //for test
        //ReadyToWriteBook("Slime1");
        //ReadyToWriteBook("Slime2");
    }
    void Update()
    {
        if (_tmpBlood != Blood) {
            _tmpBlood = Blood;
            SetBossBlood();
        }
        if (_tmpBullet != Bullet) {
            _tmpBullet = Bullet;
            SetBullet();
        }
        if (Input.GetKey(KeyCode.F)) {
            WriteBook();
        }
    }
    #region //base
    private void BossBloodAppear() {
        _bossBlood.AddToClassList("BossBlood--Opened");
    }
    private void SetBossSceneUI() {
        _bossNameText.text = BossName;
        _equippedCountText.text = _equippedCount.ToString();
        for (int i = 0; i < FullBullet; i++) {
            Vector3 bulletPosition = new Vector3(_bulletX - Margin * (FullBullet - i - 1), _bulletY, 0f);
            GameObject bulletObject = Instantiate(_bulletPrefab, _bulletCanvas.transform);
            bulletObject.GetComponent<RectTransform>().anchoredPosition = bulletPosition;
        }
    }
    private void SetBossBlood() {
        _bloodBar_Forward.transform.scale = new Vector3(((float)_tmpBlood / (float)FullBlood), 1, 1);
        //Debug.Log((float)_tmpBlood / (float)FullBlood);
    }
    private void SetBullet() {
        for (int i = 0; i < FullBullet; i++) {
            GameObject bullet = _bulletCanvas.transform.GetChild(i).gameObject;
            if (i < _tmpBullet) { bullet.GetComponent<UnityEngine.UI.Image>().color = Color.white; }
            else { bullet.GetComponent<UnityEngine.UI.Image>().color = Color.grey; }
        }
    }
    #endregion
    #region //quest
    private Stack<string> _booksToWrite = new Stack<string>();
    public void ReadyToWriteBook(string bookName) {
        if (BookData.Instance.UnlockedBookLevel[bookName] == 1) return;
        for (int i = 0; i < _questPanels.Length; i++) {
            if (_questPanels[i].ClassListContains("Panel--Opened")) continue;
            else {
                _questPanels[i].AddToClassList("Panel--Opened");
                _questPanelTexts[i].AddToClassList("PanelText--Opened");
                _booksToWrite.Push(bookName);
                break;
            }
        }
    }
    public void WriteBook() {
        if (_booksToWrite.Count == 0) return;
        _questPanels[_booksToWrite.Count - 1].RemoveFromClassList("Panel--Opened");
        _questPanelTexts[_booksToWrite.Count - 1].RemoveFromClassList("PanelText--Opened");
        BookData.Instance.UnlockedBookLevel[_booksToWrite.Pop()] = 1;
    }
    #endregion
}
