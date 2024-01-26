using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Diagnostics.Tracing;

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
        LoadSaveFile();
    }


    
    #region Public Dictionary / List
    public List<string> BookType = new List<string>() {"Asia", "Europe", "NorthAmerica", "SouthAmerica", "Africa", "Oceania"};

    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, Dictionary<string, List<string>>> BookList = new Dictionary<string, Dictionary<string, List<string>>>();
    public List<string> BookNameList = new List<string>();
    public List<string> BookNameLevelList = new List<string>();

    public Dictionary<string, int> UnlockedBookLevel = new Dictionary<string, int>();
    public Dictionary<string, int> EquippedBookLevel = new Dictionary<string, int>();
    public List<string> EquippedBook = new List<string>();
    public Dictionary<string, Dictionary<string, object>> BookDetails = new Dictionary<string, Dictionary<string, object>>();

    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int>();
    #endregion


    #region Reading Boss Data Text Files
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
            if(string.IsNullOrWhiteSpace(line) || (currentBook == "" && !BookNameLevelList.Contains(line))){
                
            }
            else if(BookNameLevelList.Contains(line)){
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
                    BookNameLevelList.Add(bookName+"_1");
                    BookNameLevelList.Add(bookName+"_2");
                    BookNameList.Add(bookName);
                    UnlockedBookLevel[bookName] = 0;
                    EquippedBookLevel[bookName] = 0;
                }
            }
        }
    }
    #endregion

    #region Reading Save Data Text Files
    private string bookSaveFilePath;
    private string[] bookSaveFileLines, fileBookList;
    private string saveFileType, fileBookName; 
    private int fileBookLevel;

    public void LoadSaveFile(){
        bookSaveFilePath = Path.Combine(Application.dataPath, "Datas", "Save", "Book Save File.txt");
        bookSaveFileLines = File.ReadAllLines(bookSaveFilePath);

        saveFileType = "";
        foreach(string line in bookSaveFileLines){
            if(string.IsNullOrWhiteSpace(line.Trim())){ saveFileType = ""; }
            else if(saveFileType == ""){
                if(line.Trim() == "Unlocked Book Level"){ saveFileType = "Unlocked Book Level"; }
                else if(line.Trim() == "Equipped Book Level"){ saveFileType = "Equipped Book Level"; }
                else if(line.Trim() == "Equipped Book"){ saveFileType = "Equipped Book"; }
            }
            else if(saveFileType == "Unlocked Book Level"){
                fileBookName = line.Split(':')[0].Trim();
                fileBookLevel = Convert.ToInt32(line.Split(':')[1].Trim());
                UnlockedBookLevel[fileBookName] = fileBookLevel;
            }
            else if(saveFileType == "Equipped Book Level"){
                fileBookName = line.Split(':')[0].Trim();
                fileBookLevel = Convert.ToInt32(line.Split(':')[1].Trim());
                EquippedBookLevel[fileBookName] = fileBookLevel;
            }
            else if(saveFileType == "Equipped Book"){
                fileBookList = line.Split(' ');
                foreach(string fileBookName in fileBookList){
                    EquippedBook.Add(fileBookName.Trim());
                }
            }
        }
    }

    private List<string> newSaveFileLines = new List<string>();
    public void UpdateSaveFile(){
        bookSaveFilePath = Path.Combine(Application.dataPath, "Datas", "Save", "Book Save File.txt");

        newSaveFileLines = new List<string>();

        newSaveFileLines.Add("Unlocked Book Level");
        foreach(string bookName in BookNameList){
            newSaveFileLines.Add(bookName + ": " + UnlockedBookLevel[bookName].ToString());
        }
        newSaveFileLines.Add("");
        
        newSaveFileLines.Add("Equipped Book Level");
        foreach(string bookName in BookNameList){
            newSaveFileLines.Add(bookName + ": " + EquippedBookLevel[bookName].ToString());
        }
        newSaveFileLines.Add("");

        string equippedBookListString = "";
        newSaveFileLines.Add("Equipped Book");
        foreach(string equippedBookString in EquippedBook){
            equippedBookListString += equippedBookString;
            equippedBookListString += " ";
        }

        File.WriteAllLines(bookSaveFilePath, newSaveFileLines.ToArray());

        GameManager.Instance.BloodManager.UpdateBloodSaveFile();
    }

    

    #endregion

}
