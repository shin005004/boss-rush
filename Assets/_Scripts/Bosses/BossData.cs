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

public class BossData : MonoBehaviour
{
    public static BossData Instance { get; private set; }    
    private string bossListFilePath, bossDetailsFilePath, bossClearFilePath; 
    private string[] bossListLines, bossDetailsLines, bossClearLines;
    private string currentType, currentBoss;

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
        bossDetailsFilePath = Path.Combine(Application.dataPath, "Datas", "Boss Details.txt");
        bossClearFilePath = Path.Combine(Application.dataPath, "Datas", "Boss Clear.txt");


        ReadFiles();
    }


    
    #region Public Dictionary / List
    public List<string> TotalBossList = new List<string>();
    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int>();
    public Dictionary<string, Dictionary<string, object>> BossDetails = new Dictionary<string, Dictionary<string, object>>();
    public Dictionary<string, bool> BossClear = new Dictionary<string, bool>();
    #endregion

    #region Reading Boss Data Text Files
    private void ReadFiles(){
        foreach(string bookType in BookData.Instance.BookType){ BossList[bookType] = new List<string>() {}; }

        bossListLines = File.ReadAllLines(bossListFilePath);
        bossDetailsLines = File.ReadAllLines(bossDetailsFilePath);
        bossClearLines = File.ReadAllLines(bossClearFilePath);

        currentType = "";
        foreach(string line in bossListLines){
            if(string.IsNullOrWhiteSpace(line) || (currentType == "" && !BookData.Instance.BookType.Contains(line))){
                
            }
            else if(BookData.Instance.BookType.Contains(line)){
                currentType = line;
            }
            else{
                BossList[currentType].Add(line.Trim());
                TotalBossList.Add(line.Trim());
            }
        }


        currentBoss = "";
        foreach(string line in bossDetailsLines){
            if(TotalBossList.Contains(line.Trim())){
                currentBoss = line;
            }
            else if(string.IsNullOrWhiteSpace(line) || currentBoss == ""){
                currentBoss = "";
            }
            else{
                if(!BossDetails.ContainsKey(currentBoss)) { BossDetails[currentBoss] = new Dictionary<string, object>(); }
                string[] detail = line.Split(':').Select(s => s.Trim()).ToArray();
                string detailKey = detail[0];
                if(detailKey == "Name"){
                    BossDetails[currentBoss].Add("Name", detail[1]);
                }
                else if(detailKey == "Health"){
                    int health = Convert.ToInt32(detail[1]);
                    BossDetails[currentBoss].Add("Health", health);
                }
                else if(detailKey == "SkillCount"){
                    int skillCount = Convert.ToInt32(detail[1]);
                    BossDetails[currentBoss].Add("SkillCount", skillCount);
                    BossSkillCount[currentBoss] = skillCount;
                }
                else if(detailKey == "Description"){
                    BossDetails[currentBoss].Add("Description", detail[1]);
                }
            }
        }

        foreach(string boss in TotalBossList){
            BossClear[boss] = false;
        }
        foreach(string line in bossClearLines){
            string[] detail = line.Split(':').Select(s => s.Trim()).ToArray();
            if(detail[1] == "O"){
                BossClear[detail[0]] = true;
            }
        }

        BookData.Instance.ReadFiles();
        BookData.Instance.LoadSaveFile();
    }
    #endregion

    private List<string> newSaveFileLines = new List<string>();
    public void UpdateBossClear(string boss){
        BossClear[boss] = true;

        newSaveFileLines = new List<string>();
        foreach(string bossString in TotalBossList){
            if(BossClear[bossString]) { newSaveFileLines.Add(bossString + ": O"); }
            else { newSaveFileLines.Add(bossString + ": X"); }
        }
    }

}
