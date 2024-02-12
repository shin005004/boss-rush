using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{
    public string BookName;
    public bool QuestFail = false, QuestSucceed = false, IsWriting = false;
    public float WaitingTime = 15.0f, WritingTime = 5.0f;
    private GameObject WaitingSlider, WritingSlider, PanelText;
    private Slider _waitingSlider, _writingSlider;
    private float _tmpWaitingTime = 0.0f, _tmpWritingTime = 0.0f;
    void Start() {
        PanelText = gameObject.transform.GetChild(0).gameObject;
        PanelText.GetComponent<TextMeshProUGUI>().text = "<color=#127A0D>[F]키</color>로 <color=#882828>" + BookName + "</color>\n책을 기록하세요";
        WaitingSlider = gameObject.transform.GetChild(1).gameObject;
        WritingSlider = gameObject.transform.GetChild(2).gameObject;
        _waitingSlider = WaitingSlider.transform.GetChild(0).GetComponent<Slider>();
        _writingSlider = WritingSlider.transform.GetChild(0).GetComponent<Slider>();
        _tmpWaitingTime = 0.0f;
        _tmpWritingTime = 0.0f;
        StartCoroutine(WaitingSliderStart());
    }
    IEnumerator WaitingSliderStart() {
        while (_tmpWaitingTime < WaitingTime) {
            _tmpWaitingTime += Time.deltaTime;
            _waitingSlider.value = (WaitingTime - _tmpWaitingTime) / WaitingTime;
            yield return null;
        }
        QuestFail = true;
    }
    IEnumerator WritingSliderStart() {
        while (_tmpWritingTime < WritingTime) {
            _tmpWritingTime += Time.deltaTime;
            _writingSlider.value = _tmpWritingTime / WritingTime;
            yield return null;
        }
        QuestSucceed = true;
    }
    public void CompleteWriting() {
        BookData.Instance.UnlockedBookLevel[BookName] = 1;
        BossSceneUI.EarnBook++;
    }
    public void WaitingToWriting() {
        StopCoroutine(nameof(WaitingSliderStart));
        _tmpWaitingTime = 0.0f;
        WaitingSlider.SetActive(false);
        WritingSlider.SetActive(true);
        IsWriting = true;
        StartCoroutine(WritingSliderStart());
    }
}
