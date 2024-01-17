using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookData
{
    public static BookData instance { get; private set; }

    public List<string> BookType = new List<string>() {"Asia", "Europe", "NorthAmerica", "SouthAmerica", "Africa", "Oceania"};
    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, List<string>>> BookList = new Dictionary<string, Dictionary<string, List<string>>>();
    public Dictionary<string, int> UnlockedBookLevel = new Dictionary<string, int>();
    public Dictionary<string, int> EquippedBookLevel = new Dictionary<string, int>();


    public void SetBossList(){
        foreach(string bookType in BookType){
            BossList[bookType] = new List<string>() {};
        }
        
        foreach(string boss in BossData.instance.AsiaBossList){
            BossList["Asia"].Add(boss);
        }
        foreach(string boss in BossData.instance.EuropeBossList){
            BossList["Europe"].Add(boss);
        }
        foreach(string boss in BossData.instance.NorthAmericaBossList){
            BossList["NorthAmerica"].Add(boss);
        }
        foreach(string boss in BossData.instance.SouthAmericaBossList){
            BossList["SouthAmerica"].Add(boss);
        }
        foreach(string boss in BossData.instance.AfricaBossList){
            BossList["Africa"].Add(boss);
        }
        foreach(string boss in BossData.instance.OceaniaBossList){
            BossList["Oceania"].Add(boss);
        }
    }

    public void SetBookList(){
        foreach(string bookType in BookType){
            BookList[bookType] = new Dictionary<string, List<string>>();
            foreach(string boss in BossList[bookType]){
                BookList[bookType][boss] = new List<string>();
                for(int i = 0; i < BossData.instance.BossSkillCount[boss]; i++){
                    string skillCount = i.ToString();
                    string bookName = $"{boss} {skillCount}";
                    BookList[bookType][boss].Add(bookName);
                    UnlockedBookLevel[bookName] = 0;
                    EquippedBookLevel[bookName] = 0;
                }
            }
        }
    }

}
