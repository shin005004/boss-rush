using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum Section {
    Index,
    Info,
    Equipped
}
public class BookUIPage : MonoBehaviour
{
    public List<string> InfoPages = new List<string>();
    public int[] Pages = {1, 1, 1};
    public int SlotNumber = 8;
    public void SetPageList() {
        foreach (string bookType in BookData.Instance.BookType) {
            foreach (string boss in BookData.Instance.BossList[bookType]) {
                    for(int i = 1; i <= BookData.Instance.BossSkillCount[boss]; i++){
                        string skillCount = i.ToString();
                        string bookName1 = $"{boss}{skillCount}_1";
                        string bookName2 = $"{boss}{skillCount}_2";
                        InfoPages.Add(bookName1);
                        InfoPages.Add(bookName2);
                }
            }
        }
        Pages[(int)Section.Index] = (InfoPages.Count - 1) / SlotNumber + 1;
        Pages[(int)Section.Info] = InfoPages.Count;
    }
}
