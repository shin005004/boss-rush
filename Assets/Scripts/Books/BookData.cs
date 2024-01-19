using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class BookData: MonoBehaviour
{
    public static BookData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        SetBossList();
        SetBookList();
    }

    public List<string> BookType = new List<string>() {"Asia", "Europe", "NorthAmerica", "SouthAmerica", "Africa", "Oceania"};
    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, List<string>>> BookList = new Dictionary<string, Dictionary<string, List<string>>>();
    public Dictionary<string, int> UnlockedBookLevel = new Dictionary<string, int>();
    public Dictionary<string, int> EquippedBookLevel = new Dictionary<string, int>();
    public List<string> EquippedBook = new List<string>();

    public void SetBossList(){
        foreach(string bookType in BookType){
            BossList[bookType] = new List<string>() {};
        }
        
        foreach(string boss in BossData.Instance.AsiaBossList){
            BossList["Asia"].Add(boss);
        }
        foreach(string boss in BossData.Instance.EuropeBossList){
            BossList["Europe"].Add(boss);
        }
        foreach(string boss in BossData.Instance.NorthAmericaBossList){
            BossList["NorthAmerica"].Add(boss);
        }
        foreach(string boss in BossData.Instance.SouthAmericaBossList){
            BossList["SouthAmerica"].Add(boss);
        }
        foreach(string boss in BossData.Instance.AfricaBossList){
            BossList["Africa"].Add(boss);
        }
        foreach(string boss in BossData.Instance.OceaniaBossList){
            BossList["Oceania"].Add(boss);
        }
    }

    public void SetBookList(){
        foreach(string bookType in BookType){
            BookList[bookType] = new Dictionary<string, List<string>>();
            foreach(string boss in BossList[bookType]){
                BookList[bookType][boss] = new List<string>();
                for(int i = 0; i < BossData.Instance.BossSkillCount[boss]; i++){
                    string skillCount = i.ToString();
                    string bookName = $"{boss} {skillCount}";
                    BookList[bookType][boss].Add(bookName);
                    UnlockedBookLevel[bookName] = 1;
                    EquippedBookLevel[bookName] = 0;
                }
            }
        }
    }

}
