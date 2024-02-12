using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    // key function = NewQuest() 
    [HideInInspector] public static List<GameObject> QuestPanels;
    [HideInInspector] public static List<string> QuestStrings;
    public GameObject PanelPrefab;
    public int QuestCount = 3;
    public float PanelX = -170f, PanelY = 110f, Margin = 200f, TransitionMargin = 50f, TransitionSpeed = 5f;
    private bool _isTransitioning = false;
    void Awake() {
        QuestPanels = new List<GameObject>(QuestCount);
        QuestStrings = new List<string>(QuestCount);
        _isTransitioning = false;
    }
    void Update()
    {
        for (int i = 0; i < QuestPanels.Count; i++) {
            if (_isTransitioning) break;
            Quest quest = QuestPanels[i].GetComponent<Quest>();
            if (quest.QuestFail) CloseQuestPanel(QuestPanels[i]);
            else if (i == 0 && quest.QuestSucceed) {
                quest.CompleteWriting();
                CloseQuestPanel(QuestPanels[i]);
            }
        }
        if (Input.GetKey(KeyCode.F) && QuestPanels.Count >= 1 && !QuestPanels[0].GetComponent<Quest>().IsWriting) {
            QuestPanels[0].GetComponent<Quest>().WaitingToWriting();
        }

        // for test
        if (Input.GetKeyUp(KeyCode.Space)) NewQuest("Thor1");
        if (Input.GetKeyUp(KeyCode.S)) NewQuest("Thor2");
    }
    public void NewQuest(string bookName) {
        if (BookData.Instance.UnlockedBookLevel[bookName] == 1) return;
        if (QuestPanels.Count == QuestCount) return;
        if (QuestStrings.Contains(bookName)) return;
        OpenQuestPanel(bookName);
    }
    private void OpenQuestPanel(string bookName) {
        GameObject questPanel = Instantiate(PanelPrefab, gameObject.transform); 
        Quest quest = questPanel.GetComponent<Quest>();
        Vector3 panelPosition = new Vector3(PanelX, PanelY + Margin * QuestPanels.Count - TransitionMargin, 0f);
        questPanel.GetComponent<Image>().color = new Color(255 / 255f, 255 / 255f, 255 / 255f, 0 / 255f);
        questPanel.GetComponent<RectTransform>().anchoredPosition = panelPosition;

        QuestPanels.Add(questPanel);
        QuestStrings.Add(bookName);
        int count = QuestPanels.IndexOf(questPanel);
        quest.BookName = bookName;

        StartCoroutine(PanelOpenTransition(questPanel, count));
    }
    private void CloseQuestPanel(GameObject panel) {
        StartCoroutine(PanelCloseTransition(panel));
    }
    private void ReadjustPanelsPosition() {
        for (int i = 0; i < QuestPanels.Count; i++) {
            Vector3 panelPosition = new Vector3(PanelX, PanelY + Margin * i, 0f);
            QuestPanels[i].GetComponent<RectTransform>().anchoredPosition = panelPosition;
        }
    }
    IEnumerator PanelOpenTransition(GameObject panel, int count) {
        while (_isTransitioning) {
            yield return null;
        }

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        Image panelImage = panel.GetComponent<Image>();
        _isTransitioning = true;

        Vector2 panelPosition = panelRect.anchoredPosition;
        Color panelColor = panelImage.color;
    
        while (panelRect.anchoredPosition.y < PanelY + Margin * count) {
            panelPosition.y += Time.deltaTime * TransitionSpeed;
            panelRect.anchoredPosition = panelPosition;
            panelColor.a += 1f / (TransitionMargin / (Time.deltaTime * TransitionSpeed));
            panelImage.color = panelColor;

            yield return null;
        }

        panelPosition.y = PanelY + Margin * count;
        panelColor.a = 1f;
        panelRect.anchoredPosition = panelPosition;
        panelImage.color = panelColor;
        panel.transform.GetChild(0).gameObject.SetActive(true);
        panel.transform.GetChild(1).gameObject.SetActive(true);

        _isTransitioning = false;
        ReadjustPanelsPosition();
    }
    IEnumerator PanelCloseTransition(GameObject panel) {
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        Image panelImage = panel.GetComponent<Image>();
        _isTransitioning = true;

        Vector2 panelPosition = panelRect.anchoredPosition;
        Color panelColor = panelImage.color;

        while (panelRect.anchoredPosition.x < - PanelX) {
            panelPosition.x += Time.deltaTime * TransitionSpeed * 8;
            panelRect.anchoredPosition = panelPosition;
            panelColor.a -= 1f / (- PanelX * 2 / (Time.deltaTime * TransitionSpeed * 8));
            panelImage.color = panelColor;
            
            yield return null;
        }
        
        QuestStrings.Remove(panel.GetComponent<Quest>().BookName);
        QuestPanels.Remove(panel);
        Destroy(panel);
        _isTransitioning = false;
        ReadjustPanelsPosition();
    }
}
