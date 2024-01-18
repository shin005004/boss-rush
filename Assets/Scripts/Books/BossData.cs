using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossData: MonoBehaviour
{
    public static BossData Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public Dictionary<string, int> BossSkillCount = new Dictionary<string, int> {
        {"AsiaBoss", 4},
        {"Thor", 4},
        {"Surtur", 4},
        {"NorthAmericaBoss", 4},
        {"SouthAmericaBoss", 4},
        {"AfricaBoss", 4},
        {"OceaniaBoss", 4}
    };

    public List<string> PassiveList = new List<string>() {

    };
    public List<string> AsiaBossList = new List<string>() {
        "AsiaBoss"
    };
    public List<string> EuropeBossList = new List<string>() {
        "Thor",
        "Surtur"
    };
    public List<string> NorthAmericaBossList = new List<string>() {
        "NorthAmericaBoss"
    };
    public List<string> SouthAmericaBossList = new List<string>() {
        "SouthAmericaBoss"
    };
    public List<string> AfricaBossList = new List<string>() {
        "AfricaBoss"
    };
    public List<string> OceaniaBossList = new List<string>() {
        "OceaniaBoss"
    };
}
