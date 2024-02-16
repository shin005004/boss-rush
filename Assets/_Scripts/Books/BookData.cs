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
    private string currentBook;
    private string bookDetailsFilePath; 
    private string[] bookDetailsLines;
    private int bloodPrice;

    #region Public Dictionary / List
    public List<string> BookType = new List<string>() {"Asia", "Europe", "NorthAmerica", "SouthAmerica", "Africa", "Oceania"};
    public Dictionary<string, Dictionary<string, List<string>>> BookList = new Dictionary<string, Dictionary<string, List<string>>>();
    public List<string> BookNameList = new List<string>();

    public Dictionary<string, int> UnlockedBookLevel = new Dictionary<string, int>();
    public Dictionary<string, int> EquippedBookLevel = new Dictionary<string, int>();
    public List<string> EquippedBook = new List<string>();
    public Dictionary<string, Dictionary<string, object>> BookDetails = new Dictionary<string, Dictionary<string, object>>();
    #endregion

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


        bookDetailsFilePath = Path.Combine(Application.streamingAssetsPath, "Datas", "Book Details.txt");


        LoadSaveFile();
    }


    


    #region Reset Equipped Books
    public void ResetEquippedBook(){
        foreach(string bookType in BookType){
            foreach(string boss in BossData.Instance.BossList[bookType]){
                for(int i = 1; i <= BossData.Instance.BossSkillCount[boss]; i++){
                    string skillCount = i.ToString();
                    string bookName = $"{boss}{skillCount}";
                    EquippedBookLevel[bookName] = 0;
                }
            }
        }
        EquippedBook = new List<string>();
    }

    #endregion

    #region Reading Boss Data Text Files
    public void ReadFiles(){
        bookDetailsLines = File.ReadAllLines(bookDetailsFilePath);

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

    private void SetBookList(){
        foreach(string bookType in BookType){
            BookList[bookType] = new Dictionary<string, List<string>>();
            foreach(string boss in BossData.Instance.BossList[bookType]){
                BookList[bookType][boss] = new List<string>();
                for(int i = 1; i <= BossData.Instance.BossSkillCount[boss]; i++){
                    string skillCount = i.ToString();
                    string bookName = $"{boss}{skillCount}";
                    BookList[bookType][boss].Add(bookName);
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
        bookSaveFilePath = Path.Combine(Application.streamingAssetsPath, "Datas", "Save", "Book Save File.txt");
        bookSaveFileLines = File.ReadAllLines(bookSaveFilePath);

        saveFileType = "";
        foreach(string line in bookSaveFileLines){
            if(string.IsNullOrWhiteSpace(line.Trim())){ saveFileType = ""; }
            else if(saveFileType == ""){
                if(line.Trim() == "Unlocked Book List"){ saveFileType = "Unlocked Book List"; }
                else if(line.Trim() == "Equipped Book List"){ saveFileType = "Equipped Book List"; }
            }
            else if(saveFileType == "Unlocked Book List"){
                fileBookName = line.Split(':')[0].Trim();
                fileBookLevel = Convert.ToInt32(line.Split(':')[1].Trim());
                UnlockedBookLevel[fileBookName] = fileBookLevel;
            }
            else if(saveFileType == "Equipped Book List"){
                fileBookName = line.Split(':')[0].Trim();
                fileBookLevel = Convert.ToInt32(line.Split(':')[1].Trim());
                EquippedBookLevel[fileBookName] = fileBookLevel;
                if (fileBookLevel == 1 && !EquippedBook.Contains(fileBookName)) EquippedBook.Add(fileBookName);
            }
        }
    }

    private List<string> newSaveFileLines = new List<string>();
    public void UpdateSaveFile(){
        bookSaveFilePath = Path.Combine(Application.dataPath, "Datas", "Save", "Book Save File.txt");

        newSaveFileLines = new List<string>();

        newSaveFileLines.Add("Unlocked Book List");
        foreach(string bookName in BookNameList){
            newSaveFileLines.Add(bookName + ": " + UnlockedBookLevel[bookName].ToString());
        }
        newSaveFileLines.Add("");
        
        newSaveFileLines.Add("Equipped Book List");
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
