using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class BookData: MonoBehaviour
{
    public static BookData Instance { get; private set; }    
    private string bossListFilePath, bossSkillCountFilePath, currentType;
    private string[] bossListLines, bossSkillCountLines;

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


        bossListFilePath = Path.Combine(Application.dataPath, "Datas", "Boss List.txt");
        bossSkillCountFilePath = Path.Combine(Application.dataPath, "Datas", "Boss Skill Count.txt");
        
        // Debug.Log(File.Exists(bossListFilePath));
        // Debug.Log(File.Exists(bossSkillCountFilePath));
        bossListLines = File.ReadAllLines(bossListFilePath);
        bossSkillCountLines = File.ReadAllLines(bossSkillCountFilePath);

        currentType = "";
        foreach(string line in bossListLines){
            if(string.IsNullOrWhiteSpace(line) || (currentType == "" && !BookType.Contains(line))){
                // Debug.Log("Invalid Line");
            }
            else if(BookType.Contains(line)){
                currentType = line;
                // Debug.Log("Type: " + line);
            }
            else{
                BossList[currentType].Add(line);
                // Debug.Log("Name: " + line);
            }
        }

        foreach(string line in bossSkillCountLines){
            string[] parts = line.Split(' ');

            if (parts.Length == 2 && int.TryParse(parts[1], out int count))
            {
                // Debug.Log(parts[0] + ": " + count.ToString());
                BossSkillCount[parts[0]] = count;
            }
        }



        SetBookList();
    }
    /*
    private void Start(){
        bossListLines = File.ReadAllLines(bossListFilePath);
        bossSkillCountLines = File.ReadAllLines(bossSkillCountFilePath);

        currentType = "";
        foreach(string line in bossListLines){
            if(string.IsNullOrWhiteSpace(line) || (currentType == "" && !BookType.Contains(line))){
                // Debug.Log("Invalid Line");
            }
            else if(BookType.Contains(line)){
                currentType = line;
                // Debug.Log("Type: " + line);
            }
            else{
                BossList[currentType].Add(line);
                // Debug.Log("Name: " + line);
            }
        }

        foreach(string line in bossSkillCountLines){
            string[] parts = line.Split(' ');

            if (parts.Length == 2 && int.TryParse(parts[1], out int count))
            {
                // Debug.Log(parts[0] + ": " + count.ToString());
                BossSkillCount[parts[0]] = count;
            }
        }



        SetBookList();
    }
    */
    public List<string> BookType = new List<string>() {"Asia", "Europe", "NorthAmerica", "SouthAmerica", "Africa", "Oceania"};
    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, List<string>>> BookList = new Dictionary<string, Dictionary<string, List<string>>>();
    public Dictionary<string, int> UnlockedBookLevel = new Dictionary<string, int>();
    public Dictionary<string, int> EquippedBookLevel = new Dictionary<string, int>();
    public List<string> EquippedBook = new List<string>();

    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int>();


    public void SetBossList(){
        foreach(string bookType in BookType){
            BossList[bookType] = new List<string>() {};
        }
    }

    public void SetBookList(){
        foreach(string bookType in BookType){
            BookList[bookType] = new Dictionary<string, List<string>>();
            foreach(string boss in BossList[bookType]){
                BookList[bookType][boss] = new List<string>();
                for(int i = 1; i <= BossSkillCount[boss]; i++){
                    string skillCount = i.ToString();
                    string bookName = $"{boss}{skillCount}";
                    BookList[bookType][boss].Add(bookName);
                    UnlockedBookLevel[bookName] = 1;
                    EquippedBookLevel[bookName] = 0;
                }
            }
        }
    }

}
