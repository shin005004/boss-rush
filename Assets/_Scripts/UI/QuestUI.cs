using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    // key function = NewQuest() 
    [HideInInspector] public static List<GameObject> QuestPanels;
    public GameObject PanelPrefab;
    public int QuestCount = 3;
    public float PanelX = -170f, PanelY = 110f, Margin = 200f;
    void Awake() {
        QuestPanels = new List<GameObject>(QuestCount);
    }
    void Update()
    {
        for (int i = 0; i < QuestPanels.Count; i++) {
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
        if (Input.GetKeyUp(KeyCode.Space)) {
            NewQuest("Thor1");
        }
        if (Input.GetKeyUp(KeyCode.A)) {
            NewQuest("Thor2");
        }
    }
    public void NewQuest(string bookName) {
        if (BookData.Instance.UnlockedBookLevel[bookName] == 1) return;
        if (QuestPanels.Count == QuestCount) return;
        OpenQuestPanel(bookName);
    }
    private void OpenQuestPanel(string bookName) {
        GameObject questPanel = Instantiate(PanelPrefab, gameObject.transform); 
        Quest quest = questPanel.GetComponent<Quest>();
        Vector3 panelPosition = new Vector3(PanelX, PanelY + Margin * QuestPanels.Count, 0f);
        questPanel.GetComponent<RectTransform>().anchoredPosition = panelPosition;

        QuestPanels.Add(questPanel);
        quest.BookName = bookName;
    }
    private void CloseQuestPanel(GameObject gameObject) {
        QuestPanels.Remove(gameObject);
        Destroy(gameObject); // Destroy GameObject

        ReadjustPanelsPosition();
    }
    private void ReadjustPanelsPosition() {
        for (int i = 0; i < QuestPanels.Count; i++) {
            Vector3 panelPosition = new Vector3(PanelX, PanelY + Margin * i, 0f);
            QuestPanels[i].GetComponent<RectTransform>().anchoredPosition = panelPosition;
        }
    }
}
