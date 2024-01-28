using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class MapUI : MonoBehaviour
{
    #region //variables

    private VisualElement _popUpLayer, _scrim, _map, _mapInfo, _closeButton, _mapInfoButton;
    private VisualElement[] _stages = new VisualElement[3];
    private Label _mapNameElement;
    private int _tmpStage = 0;
    private bool _stageChange = false;
    private List<string> _mapNames = new List<string>() {"tutorial", "Thor", "Surtur"}; // this may be expanded as data file
    private List<bool> _mapUnlocked = new List<bool>() {false, true, true}; // this may be expanded as data file
    public static bool MapAppear = false;

    #endregion
    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _popUpLayer = root.Q<VisualElement>("PopUpLayer");
        _scrim = root.Q<VisualElement>("Scrim");
        _map = root.Q<VisualElement>("Map");
        _mapInfo = root.Q<VisualElement>("MapInfo");

        for (int i = 0; i < 3; i++) {
            _stages[i] = _map.Q<VisualElement>("Stage" + i.ToString());
            _stages[i].RegisterCallback<ClickEvent, VisualElement>(OnOpenMapInfo, _stages[i]);
        }

        _closeButton = _mapInfo.Q<VisualElement>("CloseButton");
        _mapInfoButton = _mapInfo.Q<VisualElement>("MapInfoButton");
        _mapNameElement = _mapInfo.Q<Label>("MapName");

        _scrim.RegisterCallback<ClickEvent>(OnCloseMap);
        _scrim.RegisterCallback<TransitionEndEvent>(ClosePopUp);
        _closeButton.RegisterCallback<ClickEvent>(OnCloseMapInfo);
        _mapInfoButton.RegisterCallback<ClickEvent, VisualElement>(MapMove, _mapInfoButton);
        _mapInfo.RegisterCallback<TransitionEndEvent>(MapInfoStageChange);

        _popUpLayer.style.display = DisplayStyle.None;
    }
    void Update()
    {
        if (MapAppear) {
            OnOpenMap();
            MapAppear = false;
        }
        if (!GameManager.Instance.GameStateManager.UIOpened && _popUpLayer.style.display.Equals(DisplayStyle.Flex)) {
            GameManager.Instance.GameStateManager.UIOpened = true; // to handle overlap problem 
            Debug.Log(GameManager.Instance.GameStateManager.UIOpened);
        }
    }
    #region //PopUpManagement
    private void OnOpenMap() {
        _popUpLayer.style.display = DisplayStyle.Flex;
        GameManager.Instance.GameStateManager.UIOpened = true; // UI State Open

        Debug.Log(GameManager.Instance.GameStateManager.UIOpened);

        Invoke("ActiveMap", 0.1f);
    }
    private void ActiveMap() {
        _scrim.AddToClassList("Scrim--Opened");
        _map.AddToClassList("Map--Opened");
    }
    private void OnCloseMap(ClickEvent clickEvent) {
        _scrim.RemoveFromClassList("Scrim--Opened");
        _map.RemoveFromClassList("Map--Opened");
        _mapInfo.RemoveFromClassList("MapInfo--Opened");
    }
    private void ClosePopUp(TransitionEndEvent transitionEndEvent) {
        if (!_scrim.ClassListContains("Scrim--Opened")) {
            _popUpLayer.style.display = DisplayStyle.None;
            GameManager.Instance.GameStateManager.UIOpened = false; // UI State Close

            Debug.Log(GameManager.Instance.GameStateManager.UIOpened);
        }
    }
    private void OnOpenMapInfo(ClickEvent clickEvent, VisualElement visualElement) {
        if (_mapInfo.ClassListContains("MapInfo--Opened") && int.Parse(visualElement.viewDataKey) != _tmpStage) {
            _tmpStage = int.Parse(visualElement.viewDataKey);
            _mapInfo.RemoveFromClassList("MapInfo--Opened");
            _stageChange = true;
            return;
        }
        else if (!_mapInfo.ClassListContains("MapInfo--Opened")) {
            SetMapInfo();
            _mapInfo.AddToClassList("MapInfo--Opened");
        }
    }
    private void OnCloseMapInfo(ClickEvent clickEvent) {
        _mapInfo.RemoveFromClassList("MapInfo--Opened");
    }
    private void MapInfoStageChange(TransitionEndEvent transitionEndEvent) {
        if (!_mapInfo.ClassListContains("MapInfo--Opened") && _stageChange) {
            _stageChange = false;
            SetMapInfo();
            _mapInfo.AddToClassList("MapInfo--Opened");
        }
    }
    #endregion
    private void SetMapInfo() {
        // Change Map Information
        _mapInfoButton.viewDataKey = _tmpStage.ToString();
        if (_mapUnlocked[_tmpStage]) {
            _mapNameElement.text = "???";
        }
        else {
            _mapNameElement.text = _mapNames[_tmpStage];
        }
    }
    private void MapMove(ClickEvent clickEvent, VisualElement visualElement) {
        // Scene moving
        if (_mapUnlocked[_tmpStage]) {    
            Debug.Log(visualElement.viewDataKey);
        }
    }
}
