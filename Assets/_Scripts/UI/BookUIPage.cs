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
public class BookUIPage
{
    public List<string> InfoPages = new List<string>();
    public int[] Pages = {1, 1, 1};
    public int SlotNumber = 8;
    public void SetPageList() {
        InfoPages = BookData.Instance.BookNameList;
        Pages[(int)Section.Index] = (InfoPages.Count - 1) / SlotNumber + 1;
        Debug.Log(Pages[0]);
        Pages[(int)Section.Info] = InfoPages.Count;
        Debug.Log(Pages[1]);
        Pages[(int)Section.Equipped] = (BookData.Instance.EquippedBook.Count - 1) / SlotNumber + 1;
    }
}
