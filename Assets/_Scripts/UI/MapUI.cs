using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class MapUI : MonoBehaviour
{
    #region //variables
    private VisualElement _map, _closeButton, _scrim, _scrimContinent, _mapInfo;
    private VisualElement[] _continents = new VisualElement[3], _scrimContinents = new VisualElement[3];
    private int _tmpContinent;
    //
    private int[] _stageCounts = {2, 1, 0};
    private List<VisualElement> _stages = new List<VisualElement>();
    //
    private VisualElement _popUpLayer, _mapInfoButton;
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
        _map = root.Q<VisualElement>("Map");
        _closeButton = _map.Q<VisualElement>("CloseButton");
        _scrim = _map.Q<VisualElement>("Scrim");
        _scrimContinent = _map.Q<VisualElement>("ScrimContinent");
        _mapInfo = _map.Q<VisualElement>("MapInfo");

        for (int i = 0; i < _continents.Length; i++) {
            _continents[i] = _map.Q<VisualElement>("Continent" + i.ToString());
            _continents[i].RegisterCallback<ClickEvent, VisualElement>(OnOpenContinent, _continents[i]);
            _continents[i].viewDataKey = i.ToString();
            _scrimContinents[i] = _scrimContinent.Q<VisualElement>("Continent" + i.ToString());
        }

        _scrim.AddToClassList("Scrim--Closed");
        _mapInfo.AddToClassList("MapInfo--Closed");
        _scrim.style.display = DisplayStyle.None;
        _scrimContinent.style.display = DisplayStyle.None;
        _map.style.display = DisplayStyle.None;
        _scrim.RegisterCallback<ClickEvent>(OnCloseContinent);
        _scrim.RegisterCallback<TransitionEndEvent>(ContinentAppear);
        _closeButton.RegisterCallback<ClickEvent>(OnCloseMap);
        _mapInfo.RegisterCallback<TransitionEndEvent>(MapInfoStageChange);

        //
        /*
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
        _closeButton.RegisterCallback<ClickEvent>(OnCloseMapInfo);
        _mapInfoButton.RegisterCallback<ClickEvent, VisualElement>(MapMove, _mapInfoButton);

        _popUpLayer.style.display = DisplayStyle.None;
        */
    }
    void Update()
    {
        if (MapAppear) {
            OnOpenMap();
            MapAppear = false;
        }

        // for test
        if (Input.GetKeyDown(KeyCode.M)) MapAppear = true;
    }
    #region //PopUpManagement
    private void OnOpenMap() {
        _map.style.display = DisplayStyle.Flex;
        GameManager.Instance.GameStateManager.UIOpened = true;
    }
    private void OnCloseMap(ClickEvent clickEvent) {
        _map.style.display = DisplayStyle.None;
        GameManager.Instance.GameStateManager.UIOpened = false;
    }
    #endregion
    #region //Continent
    private void OnOpenContinent(ClickEvent clickEvent, VisualElement visualElement) {
        _scrim.style.display = DisplayStyle.Flex;
        _tmpContinent = int.Parse(visualElement.viewDataKey);
        SetContinent(_tmpContinent);
        _scrimContinent.style.display = DisplayStyle.Flex;
        _scrim.RemoveFromClassList("Scrim--Closed");
    }
    private void OnCloseContinent(ClickEvent clickEvent) {
        for (int i = 0; i < _scrimContinents.Length; i++) {
            if (_scrimContinents[i].style.display == DisplayStyle.Flex) {
                _scrimContinents[i].RemoveFromClassList("Continent" + i.ToString() + "--Info");
                _scrimContinents[i].RemoveFromClassList("Continent" + i.ToString() + "--Selected");
            }
        }
        _mapInfo.AddToClassList("MapInfo--Closed");
        _scrimContinent.style.display = DisplayStyle.None;
        _scrim.AddToClassList("Scrim--Closed");
        _scrim.style.display = DisplayStyle.None;
    }
    private void SetContinent(int continentIndex) {
        for (int i = 0; i < _scrimContinents.Length; i++) {
            _scrimContinents[i].style.display = DisplayStyle.None;
        }
        _scrimContinents[continentIndex].style.display = DisplayStyle.Flex;
    }
    private void ContinentAppear(TransitionEndEvent transitionEndEvent) {
        if (_scrim.ClassListContains("Scrim--Closed")) return;
        _scrimContinents[_tmpContinent].AddToClassList("Continent" + _tmpContinent + "--Selected");
        SetMapList();
    }
    #endregion
    #region //Stage
    private void SetMapList() {
        Debug.Log(_stages.Count);
        for (int i = 0; i < _stages.Count; i++) {
            _stages[i].UnregisterCallback<ClickEvent, VisualElement>(OnOpenMapInfo);
        }
        _stages.Clear();
        for (int i = 0; i < _stageCounts[_tmpContinent]; i++) {
            _stages.Add(_scrimContinents[_tmpContinent].Q<VisualElement>("Stage" + i.ToString()));
            _stages[i].RegisterCallback<ClickEvent, VisualElement>(OnOpenMapInfo, _stages[i]);
        }
    }
    private void OnOpenMapInfo(ClickEvent clickEvent, VisualElement visualElement) {
        if (!_mapInfo.ClassListContains("MapInfo--Closed") && _stages.IndexOf(visualElement) != _tmpStage) {
            _tmpStage = _stages.IndexOf(visualElement);
            _mapInfo.AddToClassList("MapInfo--Closed");
            _stageChange = true;
            return;
        }
        else if (_mapInfo.ClassListContains("MapInfo--Closed")) {
            SetMapInfo();
            _tmpStage = _stages.IndexOf(visualElement);
            _scrimContinents[_tmpContinent].AddToClassList("Continent" + _tmpContinent.ToString() + "--Info");
            _mapInfo.RemoveFromClassList("MapInfo--Closed");
        }
    }
    private void MapInfoStageChange(TransitionEndEvent transitionEndEvent) {
        if (_mapInfo.ClassListContains("MapInfo--Closed") && _stageChange) {
            _stageChange = false;
            SetMapInfo();
            _mapInfo.RemoveFromClassList("MapInfo--Closed");
        }
    }
    private void SetMapInfo() {
        // Change Map Information
        /*
        _mapInfoButton.viewDataKey = _tmpStage.ToString();
        if (_mapUnlocked[_tmpStage]) {
            _mapNameElement.text = "???";
        }
        else {
            _mapNameElement.text = _mapNames[_tmpStage];
        }
        */
    }
    private void MapMove(ClickEvent clickEvent, VisualElement visualElement) {
        // Scene moving
        /*
        if (_mapUnlocked[_tmpStage]) {    
            Debug.Log(visualElement.viewDataKey);   
        }
        if (visualElement.viewDataKey == "0") {
            GameManager.Instance.GameStateManager.UIOpened = false;
            SceneLoader.Instance.LoadBossScene();
        }
        */
    }
    #endregion
}
