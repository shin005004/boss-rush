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
    public void SetPageList(List<string> infoPages, int[] pages) {
        foreach (string bookType in BookData.Instance.BookType) {
            foreach (string boss in BookData.Instance.BossList[bookType]) {
                    for(int i = 1; i <= BookData.Instance.BossSkillCount[boss]; i++){
                        string skillCount = i.ToString();
                        string bookName1 = $"{boss}{skillCount}" + "_1";
                        string bookName2 = $"{boss}{skillCount}" + "_2";
                        infoPages.Add(bookName1);
                        infoPages.Add(bookName2);
                }
            }
        }
        pages[(int)Section.Index] = (infoPages.Count - 1) / SlotNumber + 1;
        pages[(int)Section.Info] = infoPages.Count;
    }
}
