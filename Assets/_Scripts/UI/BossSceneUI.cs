using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class BossSceneUI : MonoBehaviour
{
    #region //element
    [SerializeField] GameObject _bulletPrefab, _bulletCanvas;
    private VisualElement _bossBlood, _bloodBar_Forward, _result, _scrim;
    private Label _bossNameText, _equippedCountText, _resultText, _earnBloodText, _earnBookText;
    #endregion
    #region //variables
    public string BossName = "Boss"; // need to be connected to map data
    public static int EarnBlood = 0, EarnBook = 0;
    public int FullBlood = 200, Blood = 200; // need to be connected to map data
    public int FullBullet = 4, Bullet = 1; // need to be connected to bullet data
    public int Margin = 70; // this is for UI position adjusting
    private int _tmpBlood, _equippedCount, _tmpBullet, _bulletX = -50, _bulletY = 50, _tmpResultState;
    #endregion
    void Awake() {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _bossBlood = root.Q<VisualElement>("BossBlood");
        _bloodBar_Forward = root.Q<VisualElement>("BloodBar_Forward");
        _bossNameText = root.Q<Label>("BossName");
        _equippedCountText = root.Q<Label>("EquippedCount");

        _result = root.Q<VisualElement>("Result");
        _scrim = _result.Q<VisualElement>("Scrim");
        _resultText = _scrim.Q<Label>("ResultText");
        _earnBloodText = _scrim.Q<Label>("EarnBloodText");
        _earnBookText = _scrim.Q<Label>("EarnBookText");
        _bossBlood.RemoveFromClassList("BossBlood--Opened");
        _scrim.AddToClassList("Scrim--Closed");

        _scrim.RegisterCallback<TransitionEndEvent>(StopGame);

        EarnBlood = 0;
        EarnBook = 0;
    }
    void Start()
    {   
        GameManager.Instance.GameStateManager.UIOpened = false;
        _equippedCount = BookData.Instance.EquippedBook.Count;
        _tmpBlood = Blood;
        _tmpResultState = 0;
        SetBossBlood();
        SetBossSceneUI();

        Invoke(nameof(BossBloodAppear), 0.5f);
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
        if (GameManager.Instance.GameStateManager.ResultState != 0 && 
        GameManager.Instance.GameStateManager.ResultState != _tmpResultState) {
            _tmpResultState = GameManager.Instance.GameStateManager.ResultState;
            SetResult(GameManager.Instance.GameStateManager.ResultState);
        }
        if (GameManager.Instance.GameStateManager.UIOpened && Input.GetKeyDown(KeyCode.E)) {
            GoReborn();
        }

        // for test
        //if (Input.GetKeyDown(KeyCode.A)) GameManager.Instance.GameStateManager.ResultState = 1;
        //if (Input.GetKeyDown(KeyCode.B)) GameManager.Instance.GameStateManager.ResultState = 2;
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
    #region //result
    const string LoseText = "<color=#CA0D0D>죽었습니다</color>";
    const string WinText = "<color=#E1B41B>승리했습니다!</color>";
    private void SetResult(int resultState) {
        if (resultState == 1) _resultText.text = LoseText; // TODO: change map data
        else if (resultState == 2) {
            _resultText.text = WinText;
            BossData.Instance.BossClear[BossName] = true;
        } 
        _earnBloodText.text = "x " + EarnBlood.ToString();
        _earnBookText.text = "x " + EarnBook.ToString();
        ShowResult();
    }
    private void ShowResult() {
        GameManager.Instance.GameStateManager.UIOpened = true;
        _scrim.RemoveFromClassList("Scrim--Closed");
    }
    private void StopGame(TransitionEndEvent transitionEndEvent) {
        if (!_scrim.ClassListContains("Scrim--Closed")) Time.timeScale = 0f;
    }
    private void GoReborn() {
        GameManager.Instance.GameStateManager.UIOpened = false;
        GameManager.Instance.GameStateManager.ResultState = 0;
        _tmpResultState = 0;
        InitializeEquippedBook();
        SceneLoader.Instance.LoadMainStoreScene();
        Time.timeScale = 1f;
    }
    private void InitializeEquippedBook() {
        foreach (string bookName in BookData.Instance.EquippedBook) {
            BookData.Instance.EquippedBookLevel[bookName] = 0;
        }
        BookData.Instance.EquippedBook.Clear();
    }
    #endregion
}
