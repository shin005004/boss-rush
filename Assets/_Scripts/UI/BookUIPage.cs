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
        InfoPages = BookData.Instance.BookNameList;
        pages[(int)Section.Index] = (infoPages.Count - 1) / SlotNumber + 1;
        pages[(int)Section.Info] = infoPages.Count;
        pages[(int)Section.Equipped] = (BookData.Instance.EquippedBook.Count - 1) / SlotNumber + 1;
    }
}
