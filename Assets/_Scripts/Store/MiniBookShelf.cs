using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniBookShelf : MonoBehaviour
{
    public string destination;

    [SerializeField] private Sprite[] bookShelfSprites;
    private SpriteRenderer spriteRenderer;
    int unlockedBoss;

    private void Start(){
        spriteRenderer = GetComponent<SpriteRenderer>();

        unlockedBoss = 0;
        foreach(string boss in BookData.Instance.BossList[destination]){
            foreach(string book in BookData.Instance.BookList[destination][boss]){
                if(BookData.Instance.UnlockedBookLevel[book] > 0) { unlockedBoss++; break; }
            }
        }
        spriteRenderer.sprite = bookShelfSprites[unlockedBoss];
    }
}
