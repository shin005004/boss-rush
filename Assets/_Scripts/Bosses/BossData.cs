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
    private string bossListFilePath, bossSkillCountFilePath; 
    private string[] bossListLines, bossSkillCountLines;
    private string currentType;

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


        ReadFiles();
    }


    
    #region Public Dictionary / List

    public Dictionary<string, List<string>> BossList = new Dictionary<string, List<string>>();
    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int>();
    #endregion

    #region Reading Boss Data Text Files
    private void ReadFiles(){
        foreach(string bookType in BookData.Instance.BookType){ BossList[bookType] = new List<string>() {}; }

        bossListLines = File.ReadAllLines(bossListFilePath);
        bossSkillCountLines = File.ReadAllLines(bossSkillCountFilePath);

        currentType = "";
        foreach(string line in bossListLines){
            if(string.IsNullOrWhiteSpace(line) || (currentType == "" && !BookData.Instance.BookType.Contains(line))){
                
            }
            else if(BookData.Instance.BookType.Contains(line)){
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

        BookData.Instance.ReadFiles();
    }
    #endregion

}
