using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class BookData: MonoBehaviour
{
    public static BookData Instance { get; private set; }    
    private string bossListFilePath, bossSkillCountFilePath, bookDetailsFilePath; 
    private string currentType, currentBook;
    private string[] bossListLines, bossSkillCountLines, bookDetailsLines;
    private int bloodPrice;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        bossListFilePath = Path.Combine(Application.dataPath, "Datas", "Boss List.txt");
        bossSkillCountFilePath = Path.Combine(Application.dataPath, "Datas", "Boss Skill Count.txt");
        bookDetailsFilePath = Path.Combine(Application.dataPath, "Datas", "Book Details.txt");


        ReadFiles();
    }


    
    #region Public Dictionary / List
    public List<string> BookType = new List<string>() {"Asia", "Europe", "NorthAmerica", "SouthAmerica", "Africa", "Oceania"};

    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, List<string>>> BookList = new Dictionary<string, Dictionary<string, List<string>>>();
    public List<string> BookNameList = new List<string>();

    public Dictionary<string, int> UnlockedBookLevel = new Dictionary<string, int>();
    public Dictionary<string, int> EquippedBookLevel = new Dictionary<string, int>();
    public List<string> EquippedBook = new List<string>();
    public Dictionary<string, Dictionary<string, object>> BookDetails = new Dictionary<string, Dictionary<string, object>>();

    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int>();
    #endregion


    #region Reading Text Files
    private void ReadFiles(){
        foreach(string bookType in BookType){ BossList[bookType] = new List<string>() {}; }

        bossListLines = File.ReadAllLines(bossListFilePath);
        bossSkillCountLines = File.ReadAllLines(bossSkillCountFilePath);
        bookDetailsLines = File.ReadAllLines(bookDetailsFilePath);

        currentType = "";
        foreach(string line in bossListLines){
            if(string.IsNullOrWhiteSpace(line) || (currentType == "" && !BookType.Contains(line))){
                
            }
            else if(BookType.Contains(line)){
                currentType = line;
            }
            else{
                BossList[currentType].Add(line);
            }
        }

        foreach(string line in bossSkillCountLines){
            string[] parts = line.Split(' ');

            if (parts.Length == 2 && int.TryParse(parts[1], out int count))
            {
                BossSkillCount[parts[0]] = count;
            }
        }

        SetBookList();

        currentBook = "";
        foreach(string line in bookDetailsLines){
            if(string.IsNullOrWhiteSpace(line) || (currentBook == "" && !BookNameList.Contains(line))){
                
            }
            else if(BookNameList.Contains(line)){
                currentBook = line;
            }
            else{
                if(!BookDetails.ContainsKey(currentBook)) { BookDetails[currentBook] = new Dictionary<string, object>(); }
                string[] detail = line.Split(':').Select(s => s.Trim()).ToArray();
                string detailKey = detail[0];
                if(detailKey == "Blood"){
                    BookDetails[currentBook].Add("Blood", detail[1]);
                    // BookDetails[currentBook]["Blood"] = new List<int>() {};
                    // string[] bloodPrices = detail[1].Split(' ');
                    // foreach(string bloodPriceString in bloodPrices){
                    //     if(int.TryParse(bloodPriceString, out bloodPrice)){
                    //         ((List<int>)BookDetails[currentBook]["Blood"]).Add(bloodPrice);
                    //     }
                    // }
                }
                else if(detailKey == "Description"){
                    BookDetails[currentBook].Add("Description", detail[1]);
                }
                else if(detailKey == "Name"){
                    BookDetails[currentBook].Add("Name", detail[1]);
                }
            }
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
                    BookNameList.Add(bookName+"_1");
                    BookNameList.Add(bookName+"_2");
                    UnlockedBookLevel[bookName] = 1;
                    EquippedBookLevel[bookName] = 0;
                }
            }
        }
    }
    #endregion

}
